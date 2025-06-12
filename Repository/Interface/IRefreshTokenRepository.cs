using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByUserIdAndApp(int id, int app);
        Task<RefreshToken> GetByToken(string token);
        Task<RefreshToken> Create(RefreshToken refreshToken);
        Task<bool> Update(RefreshToken refreshToken);
        Task<bool> Delete(RefreshToken refreshToken);
    }
}
