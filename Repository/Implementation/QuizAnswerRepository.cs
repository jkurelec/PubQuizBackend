using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizAnswerDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizAnswerRepository : IQuizAnswerRepository
    {
        private readonly PubQuizContext _context;

        public QuizAnswerRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<QuizRoundResult> GradeTeamRoundAnswers(NewQuizRoundResultDto roundResultDto)
        {
            var editionResult = await _context.QuizEditionResults.FirstOrDefaultAsync(x => x.Id == roundResultDto.EditionResultId)
                ?? throw new BadRequestException("EditionResult does not exist!");

            var roundResultExists = await _context.QuizRoundResults.AnyAsync(x => x.RoundId == roundResultDto.RoundId);

            if (!roundResultExists)
                throw new BadRequestException("Result for the round already entered!");

            var round = await _context.QuizRounds
                .Include(x => x.QuizSegments)
                    .ThenInclude(x => x.QuizQuestions)
                .FirstOrDefaultAsync(x => x.Id == roundResultDto.RoundId)
                ?? throw new BadRequestException("Round not found");

            if (!SegmentResultsMatchRound(round.QuizSegments, roundResultDto.QuizSegmentResults))
                throw new BadRequestException("Segments and answers do not match all the segments and answers from the round!");

            roundResultDto.Points = GradeAnswers(round.QuizSegments, roundResultDto.QuizSegmentResults);
            editionResult.TotalPoints += roundResultDto.Points;

            return await AddTeamRoundAnswers(roundResultDto);
        }

        public async Task AuthorizeHostByRoundId(int hostId, int roundId)
        {
            var validHost = await _context.QuizRounds.AnyAsync(
                x =>
                x.Id == roundId &&
                x.Edition.HostId == hostId
            );

            if (!validHost)
                throw new UnauthorizedException();
        }

        public async Task AuthorizeHostByEditionResultId(int hostId, int editionResultId)
        {
            var validHost = await _context.QuizEditionResults.AnyAsync(
                x =>
                x.Id == editionResultId &&
                x.Edition.HostId == hostId
            );

            if (!validHost)
                throw new UnauthorizedException();
        }
        
        public async Task AuthorizeHostByEditionId(int hostId, int editionId)
        {
            var validHost = await _context.QuizEditions.AnyAsync(
                x =>
                x.Id == editionId &&
                x.HostId == hostId
            );

            if (!validHost)
                throw new UnauthorizedException();
        }

        public async Task<QuizAnswer> UpdateAnswer(QuizAnswerDetailedDto answerDto, int hostId)
        {
            var validHost = await _context.QuizAnswers.AnyAsync(
                x =>
                x.Id == answerDto.Id &&
                x.SegmentResult.Segment.Round.Edition.HostId == hostId
            );

            if (!validHost)
                throw new UnauthorizedException();

            var question = await _context.QuizQuestions.FindAsync(answerDto.QuestionId)
                ?? throw new BadRequestException($"Question {answerDto.QuestionId} not found!");

            if (!PointsValueInRange(answerDto.Points, question.Points + question.BonusPoints))
                throw new BadRequestException("Points not in range!");

            var answer = await _context.QuizAnswers.FindAsync(answerDto.Id)
                ?? throw new BadRequestException($"Answer {answerDto.Id} not found!");

            var roundResult = await _context.QuizAnswers
                .Where(x => x.Id == answer.Id)
                .Select(x => x.SegmentResult.RoundResult)
                .Include(x => x.EditionResult)
                .FirstOrDefaultAsync()
                ?? throw new BadRequestException($"Round not found!");

            roundResult.Points += answerDto.Points - answer.Points;
            roundResult.EditionResult.TotalPoints += answerDto.Points - answer.Points;

            answer.Answer = answerDto.Answer;
            answer.Points = answerDto.Points;
            SetAnswerResult(answer, question.Points);

            await _context.SaveChangesAsync();

            return answer;
        }

        public async Task<QuizSegmentResult> UpdateSegment(QuizSegmentResultDetailedDto segmentResultDto, int hostId)
        {
            var validHost = await _context.QuizSegmentResults.AnyAsync(
                x =>
                x.Id == segmentResultDto.Id &&
                x.Segment.Round.Edition.HostId == hostId
            );

            if (!validHost)
                throw new UnauthorizedException();

            var segment = await _context.QuizSegments.FindAsync(segmentResultDto.SegmentId)
                ?? throw new BadRequestException($"Segment {segmentResultDto.SegmentId} not found!");

            if (!PointsValueInRange(segmentResultDto.BonusPoints, segment.BonusPoints))
                throw new BadRequestException("Points not in range!");

            var segmentResult = await _context.QuizSegmentResults.FindAsync(segmentResultDto.Id)
                ?? throw new BadRequestException($"SegmentResult {segmentResultDto.Id} not found!");

            var roundResult = await _context.QuizSegmentResults
                .Where(x => x.Id == segmentResultDto.Id)
                .Include(x => x.RoundResult)
                    .ThenInclude(r => r.EditionResult)
                .FirstOrDefaultAsync()
                ?? throw new BadRequestException($"Round not found!");

            segmentResult.RoundResult.Points += segmentResultDto.BonusPoints - segmentResult.BonusPoints;
            segmentResult.RoundResult.EditionResult.TotalPoints += segmentResultDto.BonusPoints - segmentResult.BonusPoints;

            segmentResult.BonusPoints = segmentResultDto.BonusPoints;

            await _context.SaveChangesAsync();

            return segmentResult;
        }

        public async Task<QuizRoundResult> AddTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId)
        {
            var editionResult = await _context.QuizEditionResults.FirstOrDefaultAsync(x => x.Id == roundResultDto.EditionResultId)
                ?? throw new BadRequestException("EditionResult does not exist!");

            var roundResultExists = await _context.QuizRoundResults.AnyAsync(x => x.RoundId == roundResultDto.RoundId);

            if (!roundResultExists)
                throw new BadRequestException("Result for the round already entered!");

            var entityEntry = await _context.QuizRoundResults.AddAsync(roundResultDto.ToObject());
            editionResult.TotalPoints += roundResultDto.Points;

            await _context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async Task<QuizRoundResult> UpdateTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId)
        {
            var editionResult = await _context.QuizEditionResults.FirstOrDefaultAsync(x => x.Id == roundResultDto.EditionResultId)
                ?? throw new BadRequestException("EditionResult does not exist!");

            var roundResult = await _context.QuizRoundResults
                .Include(x => x.EditionResult)
                .FirstOrDefaultAsync(x =>
                    x.RoundId == roundResultDto.RoundId &&
                    !x.QuizSegmentResults.Any()
                )
                ?? throw new BadRequestException("Result for the round not found or has answers!");

            editionResult.TotalPoints += roundResultDto.Points - roundResult.Points;
            roundResult.Points = roundResultDto.Points;

            await _context.SaveChangesAsync();

            return roundResult;
        }

        public async Task<IEnumerable<QuizRoundResult>> GetTeamAnswers(int editionResultId, int hostId)
        {
            var editionResults = await _context.QuizRoundResults
                .Include(r => r.QuizSegmentResults)
                    .ThenInclude(s => s.QuizAnswers)
                .Where(x => x.EditionResultId == editionResultId && x.QuizSegmentResults.Any())
                .ToListAsync();

            if (editionResults.Count == 0)
                throw new NotFoundException($"Answers for EditionResult {editionResultId} not found!");

            return editionResults;
        }

        public async Task<IEnumerable<QuizEditionResult>> GetEditionResults(int editionId)
        {
            var editionResults = await _context.QuizEditionResults
                .Where(x => x.EditionId == editionId)
                .Include(x => x.Team)
                .Include(x => x.QuizRoundResults)
                .ToListAsync();

            if (editionResults.Count == 0)
                throw new NotFoundException($"No EditionResults found with edition id {editionId}!");

            return editionResults;
        }

        public async Task<IEnumerable<QuizEditionResult>> RankTeamsOnEdition(int editionId)
        {
            var editionResults = await _context.QuizEditionResults
                .Where(x => x.EditionId == editionId)
                .Include(x => x.Team)
                .Include(x => x.QuizRoundResults)
                .ToListAsync();

            if (editionResults.Count == 0)
                throw new NotFoundException($"No EditionResults found with edition id {editionId}!");

            await RankEditionResults(editionResults);

            return editionResults;
        }

        public async Task<IEnumerable<QuizEditionResult>> BreakTie(int promotedId, int editionId)
        {
            var editionResults = await GetEditionResults(editionId);

            var promotedTeam = editionResults.FirstOrDefault(x => x.Id == promotedId)
                ?? throw new BadRequestException($"No EditionResult with id => {promotedId} found!");

            var tiedTeams = editionResults.Where(x => x.Rank == promotedTeam.Rank && x.Id != promotedId && x.EditionId == editionId).ToList();

            if (tiedTeams.Count == 0)
                throw new BadRequestException("No teams tied with given team!");

            foreach (var tiedTeam in tiedTeams)
                tiedTeam.Rank--;

            await _context.SaveChangesAsync();

            return editionResults;
        }

        //KAJ SAM TU HTIO ?!?
        public async Task<int> ValidateEditionResultId(int editionId, int teamId)
        {
            var id = await _context.QuizEditionResults
                .Where(x => x.EditionId == editionId && x.TeamId == teamId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (id == default)
                throw new NotFoundException($"Team {teamId} not found in edition {editionId}");

            return id;
        }

        private static bool SegmentResultsMatchRound(IEnumerable<QuizSegment> segments, IEnumerable<NewQuizSegmentResultDto> segmentResultsDto)
        {
            var segmentsWithQuestions = segments
                .ToDictionary(
                    x => x.Id,
                    x => x.QuizQuestions.Select(q => q.Id).ToHashSet()
                );

            var segmentResultsWithAnswers = segmentResultsDto
                .ToDictionary(
                    sr => sr.SegmentId,
                    sr => sr.QuizAnswers.Select(a => a.QuestionId).ToHashSet()
                );

            bool isMatch =
                segmentsWithQuestions.Count == segmentResultsWithAnswers.Count &&
                segmentsWithQuestions.All(expected =>
                    segmentResultsWithAnswers.TryGetValue(expected.Key, out var inputQuestions) &&
                    expected.Value.SetEquals(inputQuestions)
                );

            return isMatch;
        }

        //ADAPTIRAT DA BUDE ZA SVAKOJAKE TIPOVE PITANJA I SEGMENTA
        private static decimal GradeAnswers(IEnumerable<QuizSegment> segments, IEnumerable<NewQuizSegmentResultDto> segmentDtos)
        {
            var total = 0m;

            foreach (var segment in segments)
            {
                var segmentDto = segmentDtos.FirstOrDefault(x => x.SegmentId == segment.Id)!;

                foreach (var question in segment.QuizQuestions)
                {
                    var answer = segmentDto.QuizAnswers.FirstOrDefault(x => x.QuestionId == question.Id)!;

                    total += GradeAnswer(question, answer);
                }
            }

            return total;
        }

        private static decimal GradeAnswer(QuizQuestion question, NewQuizAnswerDto answer)
        {
            var similarity = StringSimilarity.GetSimilarity(question.Answer, answer.Answer);

            if (similarity < 0.7)
            {
                answer.Points = 0;
                answer.Result = (int)QuestionResult.Incorrect;
            }   
            else if (similarity < 0.9)
            {
                answer.Points = 0.5m * question.Points;
                answer.Result = (int)QuestionResult.Parital;
            }
            else
            {
                answer.Points = question.Points;
                answer.Result = (int)QuestionResult.Correct;
            }

            return answer.Points;
        }

        private static void SetAnswerResult(QuizAnswer answer, decimal questionPoints)
        {
            if (answer.Points <= 0)
            {
                answer.Result = (int)QuestionResult.Incorrect;
            }
            else if (answer.Points < questionPoints)
            {
                answer.Result = (int)QuestionResult.Parital;
            }
            else
            {
                answer.Result = (int)QuestionResult.Correct;
            }
        }

        private static void ValidateSegmentResultsPoints(List<QuizSegment> segments, IEnumerable<NewQuizSegmentResultDto> segmentResultsDto)
        {
            foreach (var segmentResult in segmentResultsDto)
            {
                var segment = segments.Single(x => x.Id == segmentResult.SegmentId);

                if (!PointsValueInRange(segmentResult.BonusPoints, segment.BonusPoints))
                    throw new BadRequestException($"Result for segment {segment.Id} has more than possible bonus points!");

                foreach (var answer in segmentResult.QuizAnswers)
                {
                    var question = segment.QuizQuestions.Single(x => x.Id == answer.QuestionId);

                    if (!PointsValueInRange(answer.Points, question.Points + question.BonusPoints))
                        throw new BadRequestException($"Answer for question {question.Id} has more than possible points!");

                    answer.Result = GetAnswerResult(answer.Points, question.Points);
                }
            }
        }

        private static bool PointsValueInRange(decimal result, decimal range)
        {
            return Math.Abs(result) <= Math.Abs(range);
        }

        private static int GetAnswerResult(decimal answerPoints, decimal questionPoints)
        {
            if (answerPoints <= 0)
                return 0;
            else if (answerPoints < questionPoints)
                return 1;

            return 2;
        }

        private async Task<QuizRoundResult> AddTeamRoundAnswers(NewQuizRoundResultDto roundDto)
        {

            var roundResult = roundDto.ToObject();

            await _context.QuizRoundResults.AddAsync(roundResult);
            await _context.SaveChangesAsync();

            return roundResult;
        }

        private async Task RankEditionResults(List<QuizEditionResult> editionResults)
        {
            int currentRank = 1;
            int skip = 1;
            decimal? lastPoints = null;

            for (int i = 0; i < editionResults.Count; i++)
            {
                if (editionResults[i].TotalPoints != lastPoints)
                {
                    currentRank = skip;
                    lastPoints = editionResults[i].TotalPoints;
                }

                editionResults[i].Rank = currentRank;
                skip++;
            }

            await _context.SaveChangesAsync();
        }
    }
}
