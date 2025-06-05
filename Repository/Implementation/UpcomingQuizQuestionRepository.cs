using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;
using PubQuizBackend.Util.Interfaces;

namespace PubQuizBackend.Repository.Implementation
{
    public class UpcomingQuizQuestionRepository : IUpcomingQuizQuestionRepository
    {
        private readonly PubQuizContext _context;

        public UpcomingQuizQuestionRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<QuizQuestion> AddQuestion(QuizQuestionDto questionDto)
        {
            var entity = await _context.QuizQuestions.AddAsync(questionDto.ToObject());

            var round = await _context.QuizSegments
                .Where(x => x.Id == questionDto.SegmentId)
                .Select(x => x.Round)
                .Include(r => r.Edition)
                .FirstOrDefaultAsync()
                ?? throw new BadRequestException();

            round.Points += questionDto.Points;
            round.Edition.TotalPoints += questionDto.Points;

            await _context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<QuizRound> AddRound(QuizRoundDto roundDto)
        {
            var entity = await _context.AddAsync(roundDto.ToObject());
            await _context.SaveChangesAsync();

            return await GetRound(entity.Entity.Id);
        }

        public async Task<QuizSegment> AddSegment(QuizSegmentDto segmentDto)
        {
            var entity = await _context.AddAsync(segmentDto.ToObject());
            await _context.SaveChangesAsync();

            return await GetSegment(entity.Entity.Id);
        }

        public async Task DeleteQuestion(int questionId)
        {
            var question = await _context.QuizQuestions.FindAsync(questionId)
                //Moguce da nema pitanja a da je doslo do ovdje?
                ?? throw new BadRequestException("No question found");

            var segment = await _context.QuizSegments
                .Include(x => x.QuizQuestions)
                .FirstOrDefaultAsync(x => x.Id == question.SegmentId)
                ?? throw new NotFoundException("Segment not found!");

            Reorder(segment.QuizQuestions.ToList());

            var round = await _context.QuizRounds
                .Where(x => x.Id == segment.RoundId)
                .Include(r => r.Edition)
                .FirstOrDefaultAsync()
                ?? throw new BadRequestException();

            round.Points -= question.Points;
            round.Edition.TotalPoints -= question.Points;

            _context.QuizQuestions.Remove(question);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteRound(int roundId, QuizEdition edition)
        {
            var round = await _context.QuizRounds
                .Include(x => x.Edition)
                .FirstOrDefaultAsync(x => x.Id == roundId)
                ?? throw new BadRequestException("No question found");

            round.Edition.TotalPoints -= round.Points;

            _context.QuizRounds.Remove(round);

            Reorder(edition.QuizRounds.ToList());

            _context.QuizRounds.Remove(round);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSegment(int segmentId)
        {
            var segment = await _context.QuizSegments.FindAsync(segmentId)
                ?? throw new BadRequestException("No segment found");

            var roundId = segment.RoundId;

            _context.QuizSegments.Remove(segment);

            var round = await _context.QuizRounds
                .Include(x => x.QuizSegments)
                .FirstOrDefaultAsync(x => x.Id == roundId)
                    ?? throw new NotFoundException("Round not found!");

            Reorder(round.QuizSegments.ToList());

            await _context.SaveChangesAsync();
        }

        public async Task<QuizQuestion> EditQuestion(QuizQuestionDto questionDto)
        {
            var question = await _context.QuizQuestions
                .Include(x => x.Segment)
                    .ThenInclude(s => s.Round)
                        .ThenInclude(r => r.Edition)
                .FirstOrDefaultAsync(x => x.Id == questionDto.Id)
                ?? throw new BadRequestException("Question not found!");

            if (Enum.IsDefined(questionDto.Type))
            {
                if (question.Type != (int)questionDto.Type)
                    question.Type = (int)questionDto.Type;
            }
            else
                throw new BadRequestException("Invalid question type!");

            question.Segment.Round.Points += questionDto.Points - question.Points;
            question.Segment.Round.Edition.TotalPoints += questionDto.Points - question.Points;

            PropertyUpdater.UpdateEntityFromDto(question, questionDto, "Id", "SegmentId", "Number");

            await _context.SaveChangesAsync();

            return question;
        }

        public async Task<QuizSegment> EditSegment(QuizSegmentDto segmentDto)
        {
            var segment = await _context.QuizSegments.FindAsync(segmentDto.Id)
                ?? throw new BadRequestException("Question not found!");

            if (Enum.IsDefined(segmentDto.Type))
            {
                if (segment.Type != (int)segmentDto.Type)
                    segment.Type = (int)segmentDto.Type;
            }
            else
                throw new BadRequestException("Invalid question type!");

            if (segment.BonusPoints != segmentDto.BonusPoints)
                segment.BonusPoints = segmentDto.BonusPoints;

            await _context.SaveChangesAsync();

            return segment;
        }

        public async Task<QuizQuestion> GetQuestion(int questionId)
        {
            return await _context.QuizQuestions.FindAsync(questionId)
                ?? throw new NotFoundException("Question not found!");
        }

        public async Task<QuizRound> GetRound(int roundId)
        {
            var round = await _context.QuizRounds
                .Include(x => x.QuizSegments)
                    .ThenInclude(s => s.QuizQuestions)
                .FirstOrDefaultAsync(x => x.Id == roundId)
                    ?? throw new NotFoundException("Round not found!");

            round.QuizSegments = round.QuizSegments
                .OrderBy(s => s.Number)
                .ToList();

            foreach (var segment in round.QuizSegments)
                segment.QuizQuestions = segment.QuizQuestions
                    .OrderBy(q => q.Number)
                    .ToList();

            return round;
        }

        public async Task<QuizSegment> GetSegment(int segmentId)
        {
            var segment = await _context.QuizSegments
                .Include(x => x.QuizQuestions)
                .FirstOrDefaultAsync(x => x.Id == segmentId)
                    ?? throw new NotFoundException("Segment not found!");

            segment.QuizQuestions = segment.QuizQuestions
                .OrderBy(q => q.Number)
                .ToList();

            return segment;
        }

        public async Task<QuizSegment> UpdateQuestionOrder(UpdateOrderDto orderDto, QuizEdition edition)
        {
            var question = await _context.QuizQuestions.Include(x => x.Segment).ThenInclude(s => s.QuizQuestions).FirstOrDefaultAsync(x => x.Id == orderDto.Id)
                ?? throw new NotFoundException("Question not found!");

            if (orderDto.SuperId.HasValue && orderDto.SuperId.Value != question.SegmentId)
            {
                var newSegment = edition.QuizRounds.SelectMany(x => x.QuizSegments).FirstOrDefault(s => s.Id == orderDto.SuperId)
                    ?? throw new BadRequestException("Segment not in edition!");

                var newQuestions = newSegment.QuizQuestions.OrderBy(x => x.Number).ToList();

                var oldQuestions = question.Segment.QuizQuestions.OrderBy(x => x.Number).ToList();

                IsNumberValid(newQuestions.Count, orderDto.Number);

                oldQuestions.Remove(question);
                newQuestions.Insert(orderDto.Number - 1, question);

                question.SegmentId = orderDto.SuperId.Value;

                Reorder(oldQuestions);
                Reorder(newQuestions);

                await _context.SaveChangesAsync();
                
                return newSegment;
            }
            else if (question.Number != orderDto.Number)
            {
                var questions = question.Segment.QuizQuestions.OrderBy(x => x.Number).ToList();

                IsNumberValid(questions.Count, orderDto.Number);

                questions.Remove(question);
                questions.Insert(orderDto.Number - 1, question);

                Reorder(questions);

                await _context.SaveChangesAsync();

                return question.Segment;
            }

            throw new YourBadException();
        }

        public async Task<IEnumerable<QuizRound>> UpdateRoundOrder(UpdateOrderDto orderDto)
        {
            var round = await _context.QuizRounds.Include(x => x.Edition).ThenInclude(e => e.QuizRounds).FirstOrDefaultAsync(x => x.Id == orderDto.Id)
                ?? throw new NotFoundException("Round not found!");

            if (round.Number != orderDto.Number)
            {
                var rounds = round.Edition.QuizRounds.OrderBy(x => x.Number).ToList();

                IsNumberValid(rounds.Count, orderDto.Number);

                rounds.Remove(round);
                rounds.Insert(orderDto.Number - 1, round);

                Reorder(rounds);

                await _context.SaveChangesAsync();

                return round.Edition.QuizRounds;
            }

            throw new YourBadException();
        }

        public async Task<QuizRound> UpdateSegmentOrder(UpdateOrderDto orderDto, QuizEdition edition)
        {
            var segment = await _context.QuizSegments.Include(x => x.Round).ThenInclude(s => s.QuizSegments).FirstOrDefaultAsync(x => x.Id == orderDto.Id)
                ?? throw new NotFoundException("Segment not found!");

            if (orderDto.SuperId.HasValue && orderDto.SuperId.Value != segment.RoundId)
            {
                var newRound = edition.QuizRounds.FirstOrDefault(x => x.Id == orderDto.SuperId)
                    ?? throw new BadRequestException("Round not in edition!");

                var newSegments = newRound.QuizSegments.OrderBy(x => x.Number).ToList();

                var oldSegments = segment.Round.QuizSegments.OrderBy(x => x.Number).ToList();

                IsNumberValid(newSegments.Count, orderDto.Number);

                oldSegments.Remove(segment);
                newSegments.Insert(orderDto.Number - 1, segment);

                segment.RoundId = orderDto.SuperId.Value;

                Reorder(oldSegments);
                Reorder(newSegments);

                await _context.SaveChangesAsync();

                return newRound;
            }
            else if (segment.Number != orderDto.Number)
            {
                var segments = segment.Round.QuizSegments.OrderBy(x => x.Number).ToList();

                IsNumberValid(segments.Count, orderDto.Number);

                segments.Remove(segment);
                segments.Insert(orderDto.Number - 1, segment);

                Reorder(segments);

                await _context.SaveChangesAsync();

                return segment.Round;
            }

            throw new YourBadException();
        }

        public async Task<HostOrganizationQuiz> GetHost(int quizId, int userId)
        {
            return await _context.HostOrganizationQuizzes.FirstOrDefaultAsync(x => x.HostId == userId && x.QuizId == quizId)
                ?? throw new UnauthorizedException();
        }

        public async Task<QuizEdition> EditionFromQuestion(int questionId)
        {
            var question = await _context.QuizQuestions.Include(x => x.Segment).ThenInclude(s => s.Round).ThenInclude(r => r.Edition).FirstOrDefaultAsync(x => x.Id == questionId)
                ?? throw new UnauthorizedException();

            return question.Segment.Round.Edition;
        }

        public async Task<QuizEdition> EditionFromSegment(int segmentId)
        {
            var segment = await _context.QuizSegments.Include(x => x.Round).ThenInclude(r => r.Edition).FirstOrDefaultAsync(x => x.Id == segmentId)
                ?? throw new UnauthorizedException();

            return segment.Round.Edition;
        }

        public async Task<QuizEdition> EditionFromRound(int roundId)
        {
            var round = await _context.QuizRounds.Include(r => r.Edition).FirstOrDefaultAsync(x => x.Id == roundId)
                ?? throw new UnauthorizedException();

            return round.Edition;
        }

        public async Task<QuizEdition> GetEdition(int editionId)
        {
            var edition = await _context.QuizEditions.FindAsync(editionId)
                ?? throw new UnauthorizedException();

            return edition;
        }

        public void EditionHappened(QuizEdition edition)
        {
            if (edition.Time.Date < DateTime.UtcNow.Date)
                throw new BadRequestException("Quiz already happened!");
        }

        public void Reorder<T>(List<T> list) where T : INumbered
        {
            for (int i = 0; i < list.Count; i++)
                list[i].Number = i + 1;
        }

        public async Task SetQuestionNumberAndId(QuizQuestionDto questionDto)
        {
            questionDto.Number = await _context.QuizQuestions
                .Where(x => x.SegmentId == questionDto.SegmentId)
                .MaxAsync(x => (int?)x.Number) + 1
                    ?? 1;

            questionDto.Id = 0;
        }

        public async Task SetRoundNumberAndId(QuizRoundDto roundDto)
        {
            roundDto.Number = await _context.QuizRounds
                .Where(x => x.EditionId == roundDto.EditionId)
                .MaxAsync(x => (int?)x.Number) + 1
                    ?? 1;

            roundDto.Id = 0;
        }

        public async Task SetSegmentNumberAndId(QuizSegmentDto segmentDto)
        {
            segmentDto.Number = await _context.QuizSegments
                .Where(x => x.RoundId == segmentDto.RoundId)
                .MaxAsync(x => (int?)x.Number) + 1
                    ?? 1;

            segmentDto.Id = 0;
        }

        private static void IsNumberValid(int count, int number)
        {
            if (count < number || number < 1)
                throw new BadRequestException("Invalid number");
        }
    }
}
