using Microsoft.EntityFrameworkCore;
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

        public async Task<RefreshToken?> Create(RefreshToken refreshToken)
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

        public async Task<RefreshToken?> GetByToken(string token)
        {
            return await _dbContext.RefreshTokens.Include(t => t.User).Where(x => x.Token == token).FirstOrDefaultAsync();
        }

        public async Task<RefreshToken?> GetByUserId(int id)
        {
            return await _dbContext.RefreshTokens.Where(x => x.UserId == id).FirstOrDefaultAsync();
        }

        public async Task<bool?> Update(RefreshToken refreshToken)
        {
            _dbContext.Entry(refreshToken).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false; ;
        }
    }
}
