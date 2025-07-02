using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizEditionApplicationRepository : IQuizEditionApplicationRepository
    {
        private readonly PubQuizContext _context;

        public QuizEditionApplicationRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuizEditionApplication>> GetByEditionId(int id, bool? accepted = null)
        {
            return await _context.QuizEditionApplications
                .AsNoTracking()
                .Include(x => x.Team)
                .Include(x => x.Users)
                .Where(x => x.EditionId == id && x.Accepted == accepted)
                .ToListAsync();
        }

        public async Task<int> GetAcceptedCountByEditionId(int id)
        {
            return await _context.QuizEditionApplications
                .AsNoTracking()
                .Where(x => x.EditionId == id && x.Accepted == true)
                .CountAsync();
        }

        public async Task<int> GetMaxTeamsByEditionId(int id)
        {
            var edition = await _context.QuizEditions.FindAsync(id)
                ?? throw new NotFoundException($"Edition with id => {id} not found!");

            return edition.MaxTeams;
        }

        public async Task<QuizEditionApplication> GetApplicationById(int id)
        {
            return await _context.QuizEditionApplications.FindAsync(id)
                ?? throw new NotFoundException($"Application with id => {id} not found!");
        }

        public async Task<bool> CheckIfUserApplied(int userId, int editionId)
        {
            return await _context.QuizEditionApplications
                .Where(x => x.EditionId == editionId)
                .SelectMany(x => x.Users)
                .AnyAsync(u => u.Id == userId);
        }

        public async Task<QuizEditionApplication> GetApplicationByUserAndEditionId(int userId, int editionId)
        {
            return await _context.QuizEditionApplications
                .Where(x => x.EditionId == editionId && x.Users.Any(x => x.Id == userId))
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException($"Application for the edition => {editionId} not found!");
        }
    }
}
