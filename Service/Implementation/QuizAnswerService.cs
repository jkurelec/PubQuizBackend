using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.QuizAnswerDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizAnswerService : IQuizAnswerService
    {
        private readonly IQuizAnswerRepository _repository;

        public QuizAnswerService(IQuizAnswerRepository quizAnswerRepository)
        {
            _repository = quizAnswerRepository;
        }

        public async Task<QuizRoundResultDetailedDto> GradeTeamAnswers(NewQuizRoundResultDto roundDto, int hostId)
        {
            await _repository.AuthorizeHostByRoundId(hostId, roundDto.RoundId);

            if (HasAnyEmptyList(roundDto))
                throw new BadRequestException("Not all parts of the round are entered");

            var teamAnswers = await _repository.GradeTeamRoundAnswers(roundDto);

            return new (teamAnswers);
        }

        public async Task<QuizAnswerDetailedDto> UpdateAnswer(QuizAnswerDetailedDto answerDto, int hostId)
        {
            return new (await _repository.UpdateAnswer(answerDto, hostId));
        }

        public async Task<QuizSegmentResultDetailedDto> UpdateSegment(QuizSegmentResultDetailedDto segmentResultDto, int hostId)
        {
            return new(await _repository.UpdateSegment(segmentResultDto, hostId));
        }

        public static bool HasAnyEmptyList(NewQuizRoundResultDto roundDto)
        {
            if (roundDto.QuizSegmentResults == null || !roundDto.QuizSegmentResults.Any())
                return true;

            foreach (var segment in roundDto.QuizSegmentResults)
            {
                if (segment.QuizAnswers == null || !segment.QuizAnswers.Any())
                    return true;
            }

            return false;
        }

        public async Task<QuizRoundResultMinimalDto> AddTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId)
        {
            return new (await _repository.AddTeamRoundPoints(roundResultDto, hostId));
        }

        public async Task<QuizRoundResultMinimalDto> UpdateTeamRoundPoints(NewQuizRoundResultDto roundResultDto, int hostId)
        {
            return new (await _repository.UpdateTeamRoundPoints(roundResultDto, hostId));
        }

        public async Task<IEnumerable<QuizRoundResultDetailedDto>> GetTeamAnswers(int editionResultId, int hostId)
        {
            await _repository.AuthorizeHostByEditionResultId(hostId, editionResultId);

            var answers = await _repository.GetTeamAnswers(editionResultId, hostId);

            return answers.Select(x => new QuizRoundResultDetailedDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionResultBriefDto>> GetEditionResults(int editionId, int hostId)
        {
            await _repository.AuthorizeHostByEditionId(hostId, editionId);

            var editionResults = await _repository.GetEditionResults(editionId);

            return editionResults.Select(x => new QuizEditionResultBriefDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionResultBriefDto>> RankTeamsOnEdition(int editionId, int hostId)
        {
            await _repository.AuthorizeHostByEditionId(hostId, editionId);

            var editionResults = await _repository.RankTeamsOnEdition(editionId);

            return editionResults.Select(x => new QuizEditionResultBriefDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionResultBriefDto>> BreakTie(int promotedId, int editionId, int hostId)
        {
            await _repository.AuthorizeHostByEditionId(hostId, editionId);

            var editionResults = await _repository.BreakTie(promotedId, editionId);

            return editionResults.Select(x => new QuizEditionResultBriefDto(x)).ToList();
        }
    }
}
