using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IUserService
    {
        public Task<User?> Add(RegisterUserDto user);
        public Task<bool> Remove(int id);
        public Task<User?> GetById(int id);
        public Task<User?> GetByUsername(string username);
        public Task<User?> Update(int id, UserDto user);
        public Task<User?> ChangePassword(int id, string password);
        public Task<User?> GetByIdentifier(string identifier);
    }
}
