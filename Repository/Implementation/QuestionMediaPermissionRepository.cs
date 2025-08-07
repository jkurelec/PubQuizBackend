using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Model;
using PubQuizBackend.Model.Other;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuestionMediaPermissionRepository : IQuestionMediaPermissionRepository
    {
        private readonly PubQuizContext _context;

        public QuestionMediaPermissionRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<QuestionMediaPermissions> GetPermissions()
        {
            var editionData = await _context.QuizEditionResults
                .Include(er => er.Users)
                .Include(er => er.Edition)
                    .ThenInclude(e => e.Quiz)
                        .ThenInclude(q => q.HostOrganizationQuizzes)
                .Select(er => new
                {
                    er.EditionId,
                    UserIds = er.Users.Select(u => u.Id).Distinct(),
                    HostIds = er.Edition.Quiz.HostOrganizationQuizzes.Select(h => h.HostId).Distinct()
                })
                .ToListAsync();

            var dict = editionData
                .GroupBy(x => x.EditionId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var users = g.SelectMany(x => x.UserIds).Distinct().ToList();
                        var hosts = g.SelectMany(x => x.HostIds).Distinct();
                        foreach (var hostId in hosts)
                        {
                            if (!users.Contains(hostId))
                                users.Add(hostId);
                        }
                        return users;
                    });

            return new QuestionMediaPermissions { Permissions = dict };
        }
    }
}
