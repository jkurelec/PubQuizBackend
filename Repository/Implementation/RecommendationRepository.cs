using Microsoft.EntityFrameworkCore;
using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Model;
using PubQuizBackend.Model.Dto.RecommendationDto;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly PubQuizContext _context;

        public RecommendationRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<EditionFeedbackRequestDto?> GetEditionInfoForFeedback(int userId)
        {
            DateTime todayUtc = DateTime.UtcNow.Date;
            int diff = (7 + (todayUtc.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStartUtc = todayUtc.AddDays(-diff);
            DateTime weekEndUtc = weekStartUtc.AddDays(7);

            return await _context.QuizEditionResults
                .Where(x => x.Edition.Time >= weekStartUtc && x.Edition.Time < weekEndUtc)
                .Where(x => x.Users.Any(u => u.Id == userId))
                .Select(x => new EditionFeedbackRequestDto
                    {
                        EditionId = x.Edition.Id,
                        EditionName = x.Edition.Name,
                        HostUsername = x.Edition.Host.Username,
                    }
                )
                .FirstOrDefaultAsync() ?? null;
        }

        public Task<QuizEditionRecommendationParamsDto> GetEditionRecommendationParams(int editionId)
        {
            throw new NotImplementedException();
        }

        public async Task SetEditionRecommendationParams(UserFeedbackDto feedback)
        {
            //await _context.EditionRecommendationRatings.AddAsync();
        }
    }
}
