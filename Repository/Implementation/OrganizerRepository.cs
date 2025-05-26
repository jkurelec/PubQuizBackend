using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly PubQuizContext _dbContext;

        public OrganizerRepository(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Organization> Add(string name, int ownerId)
        {
            if (await _dbContext.Organizations.AnyAsync(x => x.Name == name))
                throw new ConflictException("Name already in use!");

            if (await _dbContext.Organizations.AnyAsync(x => x.OwnerId == ownerId))
                throw new YourBadException();

            var organizer = new Organization
            {
                Name = name,
                OwnerId = ownerId
            };

            await _dbContext.Organizations.AddAsync(organizer);
            await _dbContext.SaveChangesAsync();

            return await _dbContext.Organizations.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Name == name)
                ?? throw new DivineException();
        }

        public async Task<HostDto> AddHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions)
        {
            var targetUser = await _dbContext.Users.FindAsync(hostId)
                ?? throw new NotFoundException("User not found!");

            if (!Enum.TryParse<Role>(targetUser.Role.ToString(), ignoreCase: true, out var role) || role < Role.ORGANIZER)
                throw new ConflictException("User not authorized to be in an organizer!");

            if (await _dbContext.HostOrganizationQuizzes.AnyAsync(x => x.OrganizationId == organizerId && x.HostId == hostId && x.QuizId == quizId))
                throw new NotFoundException("Host already in organizer!");

            var host = new HostOrganizationQuiz
            {
                HostId = hostId,
                OrganizationId = organizerId,
                QuizId = quizId,
                CreateEdition = permissions.CreateEdition,
                EditEdition = permissions.EditEdition,
                DeleteEdition = permissions.DeleteEdition,
                CrudQuestion = permissions.CrudQuestion
            };

            await _dbContext.HostOrganizationQuizzes.AddAsync(host);
            await _dbContext.SaveChangesAsync();

            return new()
            {
                IsOwner = await IsOwner(organizerId, hostId),
                UserBrief = new()
                {
                    Id = hostId,
                },
                HostPermissions = permissions
            };
        }

        public async Task<bool> Delete(int id)
        {
            var organizer = await _dbContext.Organizations.FindAsync(id)
                ?? throw new TotalnoSiToPromislioException();

            _dbContext.Organizations.Remove(organizer);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteHost(int organizerId, int hostId)
        {
            var host = await _dbContext.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizerId && x.HostId == hostId).ToListAsync()
                    ?? throw new ConflictException("Who are you deleting?!?");

            _ = host.Select(_dbContext.HostOrganizationQuizzes.Remove);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveHostFromQuiz(int organizerId, int hostId, int quizId)
        {
            var host = await _dbContext.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizerId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                    ?? throw new ConflictException("Who are you deleting?!?");

            _dbContext.HostOrganizationQuizzes.Remove(host);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Organization> GetById(int id)
        {
            return await _dbContext.Organizations
                .Include(o => o.Owner)
                .Include(o => o.HostOrganizationQuizzes)
                    .ThenInclude(h => h.Host)
                .Include(o => o.Quizzes)
                .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Organizer not found!");
        }

        public async Task<IEnumerable<Organization>> GetAll()
        {
            return await _dbContext.Organizations
                .Include(o => o.Owner)
                .Include(o => o.HostOrganizationQuizzes)
                    .ThenInclude(h => h.Host)
                .Include(o => o.Quizzes)
                .ToListAsync()
                    ?? throw new NotFoundException("Organizer not found!");
        }

        public async Task<HostDto> GetHost(int organizerId, int hostId, int quizId)
        {
            var host = await _dbContext.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizerId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                    ?? throw new NotFoundException("Host not found!");

            return new()
            {
                IsOwner = await IsOwner(organizerId, hostId),
                UserBrief = new()
                {
                    Id = host.HostId,
                },
                HostPermissions = new()
                {
                    CreateEdition = host.CreateEdition,
                    EditEdition = host.EditEdition,
                    DeleteEdition = host.DeleteEdition,
                    CrudQuestion = host.CrudQuestion
                }
            };
        }

        public async Task<IEnumerable<HostDto>> GetHostsFromOrganization(int organizerId)
        {
            var hostOrganizers = await _dbContext.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizerId)
                .Distinct()
                .ToListAsync()
                    // jel moze biti org bez hostova? => ako su odustali pa je "arhiva" ali zas nebi ostavili nekoga???
                    ?? throw new TotalnoSiToPromislioException();

            var hosts = new List<HostDto>();

            foreach (var hostOrganizer in hostOrganizers)
            {
                var isOwner = await IsOwner(organizerId, hostOrganizer.HostId);

                var host = new HostDto
                {
                    IsOwner = isOwner,
                    UserBrief = new UserBriefDto
                    {
                        Id = hostOrganizer.HostId,
                    },
                    HostPermissions = new HostPermissionsDto
                    {
                        CreateEdition = hostOrganizer.CreateEdition,
                        EditEdition = hostOrganizer.EditEdition,
                        DeleteEdition = hostOrganizer.DeleteEdition,
                        CrudQuestion = hostOrganizer.CrudQuestion
                    }
                };

                hosts.Add(host);
            }

            return hosts;
        }

        public async Task<Organization> Update(OrganizationUpdateDto updatedOrganizer)
        {
            var organizer = await _dbContext.Organizations.FindAsync(updatedOrganizer.Id)
                ?? throw new NotFoundException("Organizer not found!");

            PropertyUpdater.UpdateEntityFromDto(organizer, updatedOrganizer);

            _dbContext.Entry(organizer).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return await GetById(organizer.Id);
        }

        public async Task<HostDto> UpdateHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions)
        {
            var host = await _dbContext.HostOrganizationQuizzes.Where(x => x.OrganizationId == organizerId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                ?? throw new NotFoundException("Host not found!");

            host.CreateEdition = permissions.CreateEdition;
            host.EditEdition = permissions.EditEdition;
            host.DeleteEdition = permissions.DeleteEdition;
            host.CrudQuestion = permissions.CrudQuestion;

            _dbContext.Entry(host).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return new()
            {
                IsOwner = await IsOwner(organizerId, hostId),
                UserBrief = new()
                {
                    Id = host.HostId,
                },
                HostPermissions = new()
                {
                    CreateEdition = host.CreateEdition,
                    EditEdition = host.EditEdition,
                    DeleteEdition = host.DeleteEdition,
                    CrudQuestion = host.CrudQuestion
                }
            };
        }

        private async Task<bool> IsOwner(int organizerId, int userId) =>
            await _dbContext.Organizations.AnyAsync(x => x.Id == organizerId && x.OwnerId == userId);
    }
}
