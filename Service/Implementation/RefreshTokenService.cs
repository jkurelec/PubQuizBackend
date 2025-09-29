using Microsoft.AspNetCore.Components.Forms;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Implementation;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using System.Security.Cryptography;

namespace PubQuizBackend.Service.Implementation
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;
        private readonly IRecommendationRepository _recommendationRepository;

        public RefreshTokenService(IRefreshTokenRepository repository, IRecommendationRepository recommendationRepository)
        {
            _repository = repository;
            _recommendationRepository = recommendationRepository;
        }

        public async Task<string> Create(int userId, int role, int app)
        {
            var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAt = DateTime.Now.AddHours(LongevityMultiplyer(role));
            var token = await _repository.GetByUserIdAndApp(userId, app);

            if (token == null)
                token = await _repository.Create(
                    new()
                    {
                        UserId = userId,
                        Token = tokenValue,
                        ExpiresAt = expiresAt,
                        App = app
                    }
                );
            else
            {
                token.Token = tokenValue;
                token.ExpiresAt = expiresAt;
            }

            await _repository.Update(token);

            var userRecommendationParams = await _recommendationRepository.GetUserRecommendationParams(userId);
            userRecommendationParams.LastLogin = DateTime.UtcNow;
            await _recommendationRepository.Save();

            return tokenValue;
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await _repository.GetByToken(token);
        }

        public async Task<bool> Delete(RefreshToken refreshToken)
        {
            return await _repository.Delete(refreshToken);
        }

        public int LongevityMultiplyer(int role) =>
            role switch
            {
                1 => 168,
                2 => 24,
                3 => 1,
                _ => 0
            };
    }
}
