using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> GetById(int id);
        Task<User?> GetByUsername(string username);
        Task<User?> GetByEmail(string email);
        Task<User> ChangePassword(int id, string password);
        Task<bool> Delete(int id);
        Task<User> Add(User user);
        Task<User> Update(UserDto user);
        Task<List<User>> GetAll();
        Task<User> GetByIdentifier(string identifier);
        Task<bool> UserExists(int id);
        Task<IEnumerable<User>> Search(string? username = null, string? sortBy = null, bool descending = false, int limit = 25);
        Task<User> GetDetailedById(int id);
        Task<string> GetUsernameById(int id);
    }
}
