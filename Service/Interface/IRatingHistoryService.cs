using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.RatingHistory;

namespace PubQuizBackend.Service.Interface
{
    public interface IRatingHistoryService
    {
        Task<List<RatingHistoryDto>> GetUserRatingHistories(int id, TimePeriod timePeriod);
        Task<List<RatingHistoryDto>> GetQuizRatingHistories(int id, TimePeriod timePeriod);
        Task AddUserRatingHistories(UserRatingHistory userRatingEntry);
        Task AddQuizRatingHistories(QuizRatingHistory quizRatingEntry);
    }
}
