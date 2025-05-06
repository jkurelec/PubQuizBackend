using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IRefreshTokenRepository
    {
        public Task<RefreshToken?> GetByUserId(int id);
        public Task<RefreshToken?> GetByToken(string token);
        public Task<RefreshToken?> Create(RefreshToken refreshToken);
        public Task<bool?> Update(RefreshToken refreshToken);
        public Task<bool> Delete(RefreshToken refreshToken);
    }
}
