using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IUserService
    {
        Task<User> Add(RegisterUserDto user);
        Task<bool> Remove(int id);
        Task<List<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByUsername(string username);
        Task<User> Update(UserDto user);
        Task<User> ChangePassword(int id, string password);
        Task<User> GetByIdentifier(string identifier);
    }
}
