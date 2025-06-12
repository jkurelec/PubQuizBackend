using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.RatingHistory;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class RatingHistoryService : IRatingHistoryService
    {
        private readonly IRatingHistoryRepository _repository;

        public RatingHistoryService(IRatingHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task AddQuizRatingHistories(QuizRatingHistory quizRatingEntry)
        {
            await _repository.AddQuizRatingHistories(quizRatingEntry);
        }

        public async Task AddUserRatingHistories(UserRatingHistory userRatingEntry)
        {
            await _repository.AddUserRatingHistories(userRatingEntry);
        }

        public async Task<List<RatingHistoryDto>> GetQuizRatingHistories(int id, TimePeriod timePeriod)
        {
            var ratingHistory = await _repository.GetQuizRatingHistories(id, timePeriod);

            return ratingHistory.Select(x => new RatingHistoryDto(x)).ToList();
        }

        public async Task<List<RatingHistoryDto>> GetUserRatingHistories(int id, TimePeriod timePeriod)
        {
            var ratingHistory = await _repository.GetUserRatingHistories(id, timePeriod);

            return ratingHistory.Select(x => new RatingHistoryDto(x)).ToList();
        }
    }
}
