using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using System.Security.Cryptography;

namespace PubQuizBackend.Service.Implementation
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _repository = refreshTokenRepository;
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
