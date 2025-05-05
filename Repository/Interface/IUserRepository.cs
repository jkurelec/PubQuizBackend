using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<UserBriefDto?> GetById(int id);
        public Task<UserBriefDto?> GetByUsername(string username);
        public Task<UserBriefDto?> GetByEmail(string email);
        public Task<bool> Delete(int id);
        public Task<UserBriefDto> Add(RegisterUserDto user);
        public Task<UserBriefDto> Update(UserDto user);
    }
}
