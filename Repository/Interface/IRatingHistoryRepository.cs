using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IRatingHistoryRepository
    {
        Task<List<UserRatingHistory>> GetUserRatingHistories(int id, TimePeriod timePeriod);
        Task<List<QuizRatingHistory>> GetQuizRatingHistories(int id, TimePeriod timePeriod);
        Task AddUserRatingHistories(UserRatingHistory userRatingEntry);
        Task AddQuizRatingHistories(QuizRatingHistory quizRatingEntry);
    }
}
