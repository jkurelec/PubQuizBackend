using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    // ZRIHVAT SVE OVE EDITION => HOST => UnauthorizedHostOrEditionHappened
    public class UpcomingQuizQuestionService : IUpcomingQuizQuestionService
    {
        private readonly IUpcomingQuizQuestionRepository _repository;

        public UpcomingQuizQuestionService(IUpcomingQuizQuestionRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuizQuestionDto> AddQuestion(QuizQuestionDto questionDto, int userId)
        {
            var edition = await _repository.EditionFromSegment(questionDto.SegmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.SetQuestionNumberAndId(questionDto);

            return new(await _repository.AddQuestion(questionDto));
        }

        public async Task<QuizRoundDto> AddRound(QuizRoundDto roundDto, int userId)
        {
            var edition = await _repository.GetEdition(roundDto.EditionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.SetRoundNumberAndId(roundDto);

            return new(await _repository.AddRound(roundDto));
        }

        public async Task<QuizSegmentDto> AddSegment(QuizSegmentDto segmentDto, int userId)
        {
            var edition = await _repository.EditionFromRound(segmentDto.RoundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.SetSegmentNumberAndId(segmentDto);

            return new(await _repository.AddSegment(segmentDto));
        }

        public async Task DeleteQuestion(int questionId, int userId)
        {
            var edition = await _repository.EditionFromQuestion(questionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.DeleteQuestion(questionId);
        }

        public async Task DeleteRound(int roundId, int userId)
        {
            var edition = await _repository.EditionFromRound(roundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.DeleteRound(roundId, edition);
        }

        public async Task DeleteSegment(int segmentId, int userId)
        {
            var edition = await _repository.EditionFromSegment(segmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            await _repository.DeleteSegment(segmentId);
        }

        public async Task<QuizQuestionDto> EditQuestion(QuizQuestionDto questionDto, int userId)
        {
            var edition = await _repository.EditionFromSegment(questionDto.SegmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.EditQuestion(questionDto));
        }

        public async Task<QuizSegmentDto> EditSegment(QuizSegmentDto segmentDto, int userId)
        {
            var edition = await _repository.EditionFromRound(segmentDto.RoundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.EditSegment(segmentDto));
        }

        public async Task<QuizQuestionDto> GetQuestion(int questionId, int userId)
        {
            var edition = await _repository.EditionFromQuestion(questionId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.GetQuestion(questionId));
        }

        public async Task<QuizRoundDto> GetRound(int roundId, int userId)
        {
            var edition = await _repository.EditionFromRound(roundId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.GetRound(roundId));
        }

        public async Task<QuizSegmentDto> GetSegment(int segmentId, int userId)
        {
            var edition = await _repository.EditionFromSegment(segmentId);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.GetSegment(segmentId));
        }

        public async Task<QuizSegmentDto> UpdateQuestionOrder(UpdateOrderDto orderDto, int userId)
        {
            var edition = await _repository.EditionFromQuestion(orderDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.UpdateQuestionOrder(orderDto, edition));
        }

        public async Task<List<QuizRoundDto>> UpdateRoundOrder(UpdateOrderDto orderDto, int userId)
        {
            var edition = await _repository.EditionFromRound(orderDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            var rounds = await _repository.UpdateRoundOrder(orderDto);

            return rounds.Select(x => new QuizRoundDto(x)).ToList();
        }

        public async Task<QuizRoundDto> UpdateSegmentOrder(UpdateOrderDto orderDto, int userId)
        {
            var edition = await _repository.EditionFromSegment(orderDto.Id);
            var host = await _repository.GetHost(edition.QuizId, userId);

            UnauthorizedHostOrEditionHappened(host, edition);

            return new(await _repository.UpdateSegmentOrder(orderDto, edition));
        }

        private void UnauthorizedHostOrEditionHappened(HostOrganizationQuiz host, QuizEdition edition)
        {
            if (!host.CrudQuestion)
                throw new ForbiddenException();

            _repository.EditionHappened(edition);
        }
    }
}
