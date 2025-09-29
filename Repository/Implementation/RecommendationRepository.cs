using Microsoft.EntityFrameworkCore;
using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.RecommendationDto;
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
    }
}
