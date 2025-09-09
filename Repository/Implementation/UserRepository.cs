using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;
using PubQuizBackend.Util.Helpers;

namespace PubQuizBackend.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly PubQuizContext _context;

        public UserRepository(PubQuizContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<User> Add(User user)
        {
            var savedUser = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return savedUser.Entity;
        }

        public async Task<User> ChangePassword(int id, string password)
        {
            var user = await _context.Users.FindAsync(id)
                ?? throw new TotalnoSiToPromislioException();

            PasswordHelper.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> Delete(int id)
        {
            _context.Users.Remove(
                await _context.Users.FindAsync(id) ?? throw new NotFoundException($"User not found!")
            );
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FindAsync(id)
                ?? throw new NotFoundException("User not found!");
        }

        public async Task<User> GetByIdentifier(string identifier)
        {
            return await _context.Users.FirstOrDefaultAsync(
                x => x.Email == identifier || x.Username == identifier
            ) ?? throw new NotFoundException("User not found!");
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetDetailedById(int id)
        {
            return await _context.Users
                .Include(x => x.HostOrganizationQuizzes)
                    .ThenInclude(h => h.Quiz)
                .Include(x => x.QuizEditions)
                    .ThenInclude(e => e.Category)
                .Include(x => x.Teams)
                .Include(x => x.EditionResults)
                    .ThenInclude(r => r.Edition)
                        .ThenInclude(e => e.Category)
                .Include(x => x.EditionResults)
                    .ThenInclude(r => r.Team)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("User not found!");
        }

        public async Task<string> GetUsernameById(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => u.Username)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("User not found!");
        }

        public async Task<IEnumerable<User>> Search(string? username = null, string? sortBy = null, bool descending = false, int limit = 25)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(u => u.Username.Contains(username));
            }

            query = sortBy?.ToLower() switch
            {
                "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "rating" => descending ? query.OrderByDescending(u => u.Rating) : query.OrderBy(u => u.Rating),
                "username" => descending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                _ => query.OrderBy(u => u.Id)
            };

            return await query.Take(limit).ToListAsync();
        }

        public async Task<User> Update(UserDto updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.Id)
                ?? throw new NotFoundException("User not found!");

            PropertyUpdater.UpdateEntityFromDto(user, updatedUser, ["Id", "Rating", "Role"]);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(x => x.Id == id);
        }
    }
}
