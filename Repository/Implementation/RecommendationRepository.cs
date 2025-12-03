using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class RecommendationRepository : BaseRepository, IRecommendationRepository
    {
        private readonly PubQuizContext _context;

        public RecommendationRepository(PubQuizContext context) : base(context)
        {
            _context = context;
        }

        public async Task<QuizEditionRecommendationParam> GetEditionRecommendationParams(int editionId)
        {
            return await _context.QuizEditionRecommendationParams.FindAsync(editionId)
                ?? throw new NotFoundException($"Edition recommendation params for id => {editionId} not found!");
        }

        public async Task SetEditionRecommendationParams(QuizEditionRecommendationParam recommendationParams)
        {
            await _context.QuizEditionRecommendationParams.AddAsync(recommendationParams);
        }

        public async Task<UserRecommendationParam> GetUserRecommendationParams(int userId)
        {
            return await _context.UserRecommendationParams.FindAsync(userId)
                ?? throw new NotFoundException($"User recommendation params for id => {userId} not found!");
        }

        public async Task SetUserRecommendationParams(UserRecommendationParam recommendationParams)
        {
            await _context.UserRecommendationParams.AddAsync(recommendationParams);
        }

        public async Task<IEnumerable<UserTopRecommendation>> GetRecommendations(int userId)
        {
            return await _context.UserTopRecommendations
                .Where(x => x.UserId == userId && x.EditionTimestamp > DateTime.UtcNow)
                .OrderByDescending(x => x.Match)
                .Take(10)
                .ToListAsync();
        }

        public async Task DeleteRecommendationsForPrevoiusEditions(CancellationToken cancellationToken = default)
        {
            var expiredEditionIds = await _context.UserTopRecommendations
                .Where(x => x.EditionTimestamp < DateTime.UtcNow)
                .GroupBy(x => x.EditionId)
                .Select(x => x.Key)
                .ToListAsync();

            if (expiredEditionIds.Count != 0)
            {
                var expiredRecs = await _context.UserTopRecommendations
                    .Where(r => expiredEditionIds.Contains(r.EditionId))
                    .ToListAsync();

                _context.UserTopRecommendations.RemoveRange(expiredRecs);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
