using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Service.Interface
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GetByToken(string token);
        Task<string> Create(int userId, int role, int app);
        Task<bool> Delete(RefreshToken refreshToken);
    }
}
