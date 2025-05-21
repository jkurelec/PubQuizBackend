using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<User> GetById(int id);
        public Task<User?> GetByUsername(string username);
        public Task<User?> GetByEmail(string email);
        public Task<User> ChangePassword(int id, string password);
        public Task<bool> Delete(int id);
        public Task<User> Add(RegisterUserDto user);
        public Task<User> Update(UserDto user);
        public Task<List<User>> GetAll();
        public Task<User> GetByIdentifier(string identifier);
    }
}
