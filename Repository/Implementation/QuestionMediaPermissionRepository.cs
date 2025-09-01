using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Model;
using PubQuizBackend.Model.Other;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuestionMediaPermissionRepository : IQuestionMediaPermissionRepository
    {
        private static QuestionMediaPermissions _permissions = new();
        private static bool _permissionsSet = false;

        public QuestionMediaPermissions GetPermissions()
        {
            return _permissionsSet
                ? _permissions
                : new QuestionMediaPermissions();
        }

        public async Task SetPermissions(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PubQuizContext>();

            var editionUsers = await context.QuizEditionResults
                .SelectMany(x => x.Users.Select(u => new { x.EditionId, u.Id }))
                .ToListAsync();

            var editionHosts = await context.QuizEditions
                .Include(x => x.Quiz)
                .SelectMany(x => x.Quiz.HostOrganizationQuizzes.Select(h => new { x.Id, h.HostId }))
                .ToListAsync();

            foreach (var editionUser in editionUsers)
            {
                if (!_permissions.Permissions.TryGetValue(editionUser.EditionId, out var set))
                {
                    set = new HashSet<int>();
                    _permissions.Permissions[editionUser.EditionId] = set;
                }

                set.Add(editionUser.Id);
            }

            foreach (var editionHost in editionHosts)
            {
                if (!_permissions.Permissions.TryGetValue(editionHost.Id, out var set))
                {
                    set = new HashSet<int>();
                    _permissions.Permissions[editionHost.Id] = set;
                }

                set.Add(editionHost.HostId);
            }

            _permissionsSet = true;
        }
    }
}
