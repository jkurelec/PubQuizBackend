using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PubQuizBackend.Auth;
using PubQuizBackend.Model;
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

        public async Task<UserBriefDto> Add(RegisterUserDto user)
        {
            if (!await _dbContext.Users.AnyAsync(x => x.Username == registerUserDto.Username || x.Email == registerUserDto.Email))
            {
                await _dbContext.Users.AddAsync(registerUserDto.ToUser());
                await _dbContext.SaveChangesAsync();
            }
            else
                return BadRequest("Username or Email already taken!");

            var user = await _dbContext.Users.Where(x => x.Username == registerUserDto.Username).FirstOrDefaultAsync();

            if (user != null)
            {
                var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, CustomConverter.GetStringRole(user.Role));
                var refreshToken = _jwtService.GenerateRefreshToken(user.Id, user.Role);

                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }

            return BadRequest("Something went wrong!");
        }

        public async Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserBriefDto?> GetByEmail(string email)
        {
            var user = await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

            if (user != null)
                return new()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Rating = user.Rating
                };
            
            return null;
        }

        public async Task<UserBriefDto?> GetById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user != null)
                return new()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Rating = user.Rating
                };

            return null;
        }

        public async Task<UserBriefDto?> GetByUsername(string username)
        {
            var user = await _dbContext.Users.Where(x => x.Username == username).FirstOrDefaultAsync();

            if (user != null)
                return new()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Rating = user.Rating
                };

            return null;
        }

        public async Task<UserBriefDto> Update(int id, UserDto updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(id);

            PropertyUpdater.UpdateEntityFromDto(user, updatedUser);

            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return null;
        }
    }
}
