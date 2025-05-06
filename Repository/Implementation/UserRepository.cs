using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PubQuizBackend.Auth;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;
using PubQuizBackend.Utils;

namespace PubQuizBackend.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly PubQuizContext _dbContext;

        public UserRepository(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> Add(RegisterUserDto userinfo)
        {
            if (!await UsernameOrEmailExists(userinfo.Username, userinfo.Email))
            {
                var user = await _dbContext.Users.AddAsync(userinfo.ToUser());
                await _dbContext.SaveChangesAsync();

                return user.Entity;
            }

            return null;
        }

        public async Task<User?> ChangePassword(int id, string password)
        {
            var user = await _dbContext.Users.FindAsync(id);

            user.Password = password;

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> Delete(int id)
        {
            _dbContext.Users.Remove(_dbContext.Users.Find(id));
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            var user = await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

            if (user != null)
                return user;
            
            return null;
        }

        public async Task<User?> GetById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user != null)
                return user;

            return null;
        }

        public async Task<User?> GetByIdentifier(string identifier)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(
                x => x.Email == identifier || x.Username == identifier
            );
        }

        public async Task<User?> GetByUsername(string username)
        {
            var user = await _dbContext.Users.Where(x => x.Username == username).FirstOrDefaultAsync();

            if (user != null)
                return user;

            return null;
        }

        public async Task<User?> Update(int id, UserDto updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if(user != null)
            {
                PropertyUpdater.UpdateEntityFromDto(user, updatedUser, ["Id", "Rating", "Role"]);

                _dbContext.Entry(user).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return user;
            }

            return null;
        }

        protected async Task<bool> UsernameOrEmailExists(string username, string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Username == username || x.Email == email);
        }
    }
}
