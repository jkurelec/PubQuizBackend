using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly PubQuizContext _dbContext;

        public RefreshTokenRepository(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken> Create(RefreshToken refreshToken)
        {
            await _dbContext.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<bool> Delete(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await _dbContext.RefreshTokens.Include(t => t.User).Where(x => x.Token == token).FirstOrDefaultAsync()
                ?? throw new UnauthorizedException();
        }

        public async Task<RefreshToken?> GetByUserIdAndApp(int id, int app)
        {
            return await _dbContext.RefreshTokens.Where(x => x.UserId == id && x.App == app).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(RefreshToken refreshToken)
        {
            _dbContext.Entry(refreshToken).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
