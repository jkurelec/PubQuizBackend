
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific;

namespace PubQuizBackend.Service.Interface
{
    public interface IUpcomingQuizQuestionService
    {
        Task<QuizQuestionDto> AddQuestion(QuizQuestionDto questionDto, int userId);
        Task<QuizSegmentDto> AddSegment(QuizSegmentDto segmentDto, int userId);
        Task<QuizRoundDto> AddRound(QuizRoundDto roundDto, int userId);
        Task<QuizQuestionDto> EditQuestion(QuizQuestionDto questionDto, int userId);
        Task<QuizSegmentDto> EditSegment(QuizSegmentDto segmentDto, int userId);
        Task DeleteQuestion(int questionId, int userId);
        Task DeleteSegment(int segmentId, int userId);
        Task DeleteRound(int roundId, int userId);
        Task<QuizQuestionDto> GetQuestion(int questionId, int userId);
        Task<QuizSegmentDto> GetSegment(int segmentId, int userId);
        Task<QuizRoundDto> GetRound(int roundId, int userId);
        Task<QuizSegmentDto> UpdateQuestionOrder(UpdateOrderDto orderDto, int userId);
        Task<QuizRoundDto> UpdateSegmentOrder(UpdateOrderDto orderDto, int userId);
        Task<List<QuizRoundDto>> UpdateRoundOrder(UpdateOrderDto orderDto, int userId);
    }
}
