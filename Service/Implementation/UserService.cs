using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;
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

        public async Task<User> Add(User user)
        {
            return await _repository.Add(user);
        }

        public async Task<User> ChangePassword(int id, string password)
        {
            return await _repository.ChangePassword(id, password);
        }

        public async Task ExistsByUsernameOrEmail(string username, string email)
        {
            if (await _repository.GetByUsername(username) != null || await _repository.GetByEmail(email) != null)
                throw new ConflictException("Username or email already in use!");
        }

        public async Task<List<User>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<User> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<User> GetByIdentifier(string identifier)
        {
            return await _repository.GetByIdentifier(identifier);
        }

        public async Task<User> GetByUsername(string username)
        {
            return await _repository.GetByUsername(username)
                ?? throw new NotFoundException("User not found!");
        }

        public async Task<UserDetailedDto> GetDetailedById(int id)
        {
            var user = await _repository.GetDetailedById(id);

            var hostingQuizzes = user.HostOrganizationQuizzes
                .Select(x => x.Quiz)
                .Select(x => new QuizMinimalDto(x))
                .ToList();

            return new UserDetailedDto(user, hostingQuizzes);
        }

        public async Task<bool> Remove(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<IEnumerable<UserBriefDto>> Search(string? username = null, string? sortBy = null, bool descending = false, int limit = 25)
        {
            var users = await _repository.Search(username, sortBy, descending, limit);

            return users.Select(x => new UserBriefDto(x)).ToList();
        }

        public async Task<User> Update(UserDto user)
        {
            return await _repository.Update(user);
        }
    }
}
