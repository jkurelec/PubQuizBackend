using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Enums;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Repository.Implementation
{
    public class RatingHistoryRepository : IRatingHistoryRepository
    {
        private readonly PubQuizContext _context;

        public RatingHistoryRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task AddQuizRatingHistories(QuizRatingHistory quizRatingEntry)
        {
            await _context.QuizRatingHistories.AddAsync(quizRatingEntry);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserRatingHistories(UserRatingHistory userRatingEntry)
        {
            await _context.UserRatingHistories.AddAsync(userRatingEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<List<QuizRatingHistory>> GetQuizRatingHistories(int id, TimePeriod timePeriod)
        {
            return await _context.QuizRatingHistories
                .Where(x =>
                    x.QuizId == id &&
                    x.Date > timePeriod.GetStartDateUtc()
                )
                .ToListAsync();
        }

        public async Task<List<UserRatingHistory>> GetUserRatingHistories(int id, TimePeriod timePeriod)
        {
            var jebeniPeriod = timePeriod.GetStartDateUtc();
            return await _context.UserRatingHistories
                .Where(x =>
                    x.UserId == id &&
                    x.Date > timePeriod.GetStartDateUtc()
                )
                .ToListAsync();
        }
    }
}
