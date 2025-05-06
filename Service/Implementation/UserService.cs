using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> Add(RegisterUserDto user)
        {
            return await _repository.Add(user);
        }

        public async Task<User?> ChangePassword(int id, string password)
        {
            return await _repository.ChangePassword(id, password);
        }

        public async Task<User?> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<User?> GetByIdentifier(string identifier)
        {
            return await _repository.GetByIdentifier(identifier);
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _repository.GetByUsername(username);
        }

        public async Task<bool> Remove(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<User?> Update(int id, UserDto user)
        {
            return await _repository.Update(id, user);
        }
    }
}
