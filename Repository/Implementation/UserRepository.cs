using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly PubQuizContext _dbContext;

        public UserRepository(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> Add(RegisterUserDto userinfo)
        {
            var user = await _dbContext.Users.AddAsync(userinfo.ToUser());
            await _dbContext.SaveChangesAsync();

            return user.Entity;
        }

        public async Task<User> ChangePassword(int id, string password)
        {
            var user = await _dbContext.Users.FindAsync(id)
                ?? throw new TotalnoSiToPromislioException();

            user.Password = password;

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> Delete(int id)
        {
            _dbContext.Users.Remove(
                await _dbContext.Users.FindAsync(id) ?? throw new NotFoundException($"User not found!")
            );
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _dbContext.Users.FindAsync(id)
                ?? throw new NotFoundException("User not found!");
        }

        public async Task<User> GetByIdentifier(string identifier)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(
                x => x.Email == identifier || x.Username == identifier
            ) ?? throw new NotFoundException("User not found!");
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _dbContext.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> Update(UserDto updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(updatedUser.Id)
                ?? throw new NotFoundException("User not found!");

            PropertyUpdater.UpdateEntityFromDto(user, updatedUser, ["Id", "Rating", "Role"]);

            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return user;
        }
    }
}
