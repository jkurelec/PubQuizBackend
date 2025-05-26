using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific;
using PubQuizBackend.Util.Interfaces;

namespace PubQuizBackend.Repository.Interface
{
    public interface IUpcomingQuizQuestionRepository
    {
        Task<QuizQuestion> AddQuestion(QuizQuestionDto questionDto);
        Task<QuizSegment> AddSegment(QuizSegmentDto segmentDto);
        Task<QuizRound> AddRound(QuizRoundDto roundDto);
        Task<QuizQuestion> EditQuestion(QuizQuestionDto questionDto);
        Task<QuizSegment> EditSegment(QuizSegmentDto segmentDto);
        Task DeleteQuestion(int questionId);
        Task DeleteSegment(int segmentId);
        Task DeleteRound(int roundId, QuizEdition edition);
        Task<QuizQuestion> GetQuestion(int questionId);
        Task<QuizSegment> GetSegment(int segmentId);
        Task<QuizRound> GetRound(int roundId);
        Task<QuizEdition> GetEdition(int editionId);
        Task<QuizSegment> UpdateQuestionOrder(UpdateOrderDto orderDto, QuizEdition edition);
        Task<QuizRound> UpdateSegmentOrder(UpdateOrderDto orderDto, QuizEdition edition);
        Task<IEnumerable<QuizRound>> UpdateRoundOrder(UpdateOrderDto orderDto);
        Task<HostOrganizationQuiz> GetHost(int quizId, int userId);
        Task<QuizEdition> EditionFromQuestion(int questionId);
        Task<QuizEdition> EditionFromSegment(int segmentId);
        Task<QuizEdition> EditionFromRound(int roundId);
        Task SetQuestionNumberAndId(QuizQuestionDto questionDto);
        Task SetRoundNumberAndId(QuizRoundDto roundDto);
        Task SetSegmentNumberAndId(QuizSegmentDto segmentDto);
        void EditionHappened(QuizEdition edition);
        void Reorder<T>(List<T> list) where T : INumbered;
    }
}
