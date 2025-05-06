using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Service.Interface
{
    public interface IRefreshTokenService
    {
        public Task<RefreshToken?> GetByUserId(int id);
        public Task<RefreshToken?> GetByToken(string token);
        public Task<string?> Create(int userId, int role);
        public Task<bool> Delete(RefreshToken refreshToken);
    }
}
