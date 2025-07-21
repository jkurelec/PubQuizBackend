using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IUserService
    {
        Task<User> Add(User user);
        Task<bool> Remove(int id);
        Task<List<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByUsername(string username);
        Task<User> Update(UserDto user);
        Task<User> ChangePassword(int id, string password);
        Task<User> GetByIdentifier(string identifier);
        Task ExistsByUsernameOrEmail(string username, string email);
        Task<IEnumerable<UserBriefDto>> Search(string? username = null, string? sortBy = null, bool descending = false, int limit = 25);
        Task<UserDetailedDto> GetDetailedById(int id);
    }
}
