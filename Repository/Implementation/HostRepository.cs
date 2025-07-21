using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Model;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class HostRepository : IHostRepository
    {
        private readonly PubQuizContext _context;

        public HostRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<int>> GetQuizIdsByHostAndOrganization(int hostId, int organizationId)
        {
            return await _context.HostOrganizationQuizzes
                .Where(x =>
                    x.HostId == hostId
                    && x.OrganizationId == organizationId
                )
                .Select(x => x.QuizId)
                .ToListAsync();
        }
    }
}
