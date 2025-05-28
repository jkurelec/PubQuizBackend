using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly PubQuizContext _context;

        public OrganizationRepository(PubQuizContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<bool> IsNameInUse(string name)
        {
            return await _context.Organizations.AnyAsync(x => x.Name == name);
        }

        public async Task<bool> IsUserOwner(int userId)
        {
            return await _context.Organizations.AnyAsync(x => x.OwnerId == userId);
        }

        public async Task<Organization> Add(string name, int ownerId)
        {
            var organizer = new Organization
            {
                Name = name,
                OwnerId = ownerId
            };

            var organizerEntityEntry = await _context.Organizations.AddAsync(organizer);
            await _context.SaveChangesAsync();

            return await GetByIdForBriefDto(organizerEntityEntry.Entity.Id);
        }

        public async Task<bool> IsHostInQuiz(int organizationId, int hostId, int quizId)
        {
            return await _context.HostOrganizationQuizzes.AnyAsync(x => x.OrganizationId == organizationId && x.HostId == hostId && x.QuizId == quizId);
        }

        public async Task<bool> DoesHostExist(int hostId, int quizId)
        {
            return await _context.HostOrganizationQuizzes.AnyAsync(x => x.HostId == hostId && x.QuizId == quizId);
        }
        

        public async Task<HostDto> AddHost(int organizationId, int hostId, int quizId, HostPermissionsDto permissions)
        {
            if (await _context.HostOrganizationQuizzes.AnyAsync(x => x.OrganizationId == organizationId && x.HostId == hostId && x.QuizId == quizId))
                throw new NotFoundException("Host already in organizer!");

            var host = new HostOrganizationQuiz
            {
                HostId = hostId,
                OrganizationId = organizationId,
                QuizId = quizId,
                CreateEdition = permissions.CreateEdition,
                EditEdition = permissions.EditEdition,
                DeleteEdition = permissions.DeleteEdition,
                CrudQuestion = permissions.CrudQuestion,
                ManageApplication = permissions.ManageApplication
            };

            await _context.HostOrganizationQuizzes.AddAsync(host);
            await _context.SaveChangesAsync();

            return new()
            {
                IsOwner = await IsOwner(organizationId, hostId),
                UserBrief = new()
                {
                    Id = hostId,
                },
                HostPermissions = permissions
            };
        }

        public async Task Delete(Organization organization)
        {
            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHost(int organizationId, int hostId)
        {
            var host = await _context.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizationId && x.HostId == hostId).ToListAsync()
                    ?? throw new ConflictException("Who are you deleting?!?");

            _context.HostOrganizationQuizzes.RemoveRange(host);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveHostFromQuiz(int organizationId, int hostId, int quizId)
        {
            var host = await _context.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizationId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                    ?? throw new ConflictException("Who are you deleting?!?");

            _context.HostOrganizationQuizzes.Remove(host);
            await _context.SaveChangesAsync();
        }

        public async Task<Organization> GetById(int id)
        {
            return await _context.Organizations.FindAsync(id)
                ?? throw new NotFoundException($"Organization {id} not found!");
        }

        public async Task<Organization> GetByIdForBriefDto(int id)
        {
            return await _context.Organizations
                .Include(o => o.Owner)
                .Include(o => o.Quizzes)
                .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Organization not found!");
        }

        public async Task<Organization> GetByIdForDetailedDto(int id)
        {
            return await _context.Organizations
                .Include(o => o.Owner)
                .Include(o => o.HostOrganizationQuizzes)
                    .ThenInclude(h => h.Host)
                .Include(o => o.Quizzes)
                .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Organization not found!");
        }

        public async Task<IEnumerable<Organization>> GetAll()
        {
            return await _context.Organizations
                .Include(o => o.Owner)
                .Include(o => o.HostOrganizationQuizzes)
                    .ThenInclude(h => h.Host)
                .Include(o => o.Quizzes)
                .ToListAsync()
                    ?? throw new NotFoundException("Organizer not found!");
        }

        public async Task<HostOrganizationQuiz> GetHostByEditionId(int hostId, int editionId)
        {
            return await _context.HostOrganizationQuizzes.FirstOrDefaultAsync(
                x => x.HostId == hostId && x.Quiz.QuizEditions.Any(e => e.Id == editionId)
            ) ?? throw new NotFoundException($"Host {hostId} for edition {editionId} not found!");
        }

        public async Task<HostOrganizationQuiz> GetHost(int hostId, int quizId)
        {
            return await _context.HostOrganizationQuizzes.FirstOrDefaultAsync(
                x => x.HostId == hostId && x.QuizId == quizId
            ) ?? throw new NotFoundException($"Host {hostId} for quiz {quizId} not found!");
        }

        // ovdje tehnicki nije potreban organizationId
        public async Task<HostDto> GetHostDto(int organizationId, int hostId, int quizId)
        {
            var host = await _context.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizationId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                    ?? throw new NotFoundException("Host not found!");

            return new()
            {
                IsOwner = await IsOwner(organizationId, hostId),
                UserBrief = new()
                {
                    Id = host.HostId,
                },
                HostPermissions = new()
                {
                    CreateEdition = host.CreateEdition,
                    EditEdition = host.EditEdition,
                    DeleteEdition = host.DeleteEdition,
                    CrudQuestion = host.CrudQuestion,
                    ManageApplication = host.ManageApplication
                }
            };
        }

        public async Task<IEnumerable<HostDto>> GetHostsFromOrganization(int organizationId)
        {
            var hostOrganizers = await _context.HostOrganizationQuizzes
                .Where(x => x.OrganizationId == organizationId)
                .Distinct()
                .ToListAsync()
                    // jel moze biti org bez hostova? => ako su odustali pa je "arhiva" ali zas nebi ostavili nekoga???
                    ?? throw new TotalnoSiToPromislioException();

            var hosts = new List<HostDto>();

            foreach (var hostOrganizer in hostOrganizers)
            {
                var isOwner = await IsOwner(organizationId, hostOrganizer.HostId);

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
                        CrudQuestion = hostOrganizer.CrudQuestion,
                        ManageApplication = hostOrganizer.ManageApplication
                    }
                };

                hosts.Add(host);
            }

            return hosts;
        }

        public async Task<Organization> Update(OrganizationUpdateDto updatedOrganization)
        {
            var organizer = await _context.Organizations.FindAsync(updatedOrganization.Id)
                ?? throw new NotFoundException("Organizer not found!");

            PropertyUpdater.UpdateEntityFromDto(organizer, updatedOrganization);

            _context.Entry(organizer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return await GetById(organizer.Id);
        }

        public async Task<HostDto> UpdateHost(int organizationId, int hostId, int quizId, HostPermissionsDto permissions)
        {
            var host = await _context.HostOrganizationQuizzes.Where(x => x.OrganizationId == organizationId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                ?? throw new NotFoundException("Host not found!");

            host.CreateEdition = permissions.CreateEdition;
            host.EditEdition = permissions.EditEdition;
            host.DeleteEdition = permissions.DeleteEdition;
            host.CrudQuestion = permissions.CrudQuestion;
            host.ManageApplication = permissions.ManageApplication;

            _context.Entry(host).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new()
            {
                IsOwner = await IsOwner(organizationId, hostId),
                UserBrief = new()
                {
                    Id = host.HostId,
                },
                HostPermissions = new()
                {
                    CreateEdition = host.CreateEdition,
                    EditEdition = host.EditEdition,
                    DeleteEdition = host.DeleteEdition,
                    CrudQuestion = host.CrudQuestion,
                    ManageApplication = host.ManageApplication
                }
            };
        }

        private async Task<bool> IsOwner(int organizationId, int userId) =>
            await _context.Organizations.AnyAsync(x => x.Id == organizationId && x.OwnerId == userId);
    }
}
