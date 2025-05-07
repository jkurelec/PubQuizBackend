using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;
using PubQuizBackend.Model.Dto.OrganizerDto;
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

        public async Task<Organizer?> Add(string name, int ownerId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var organizer = new Organizer {
                    Name = name,
                    OwnerId = ownerId
                };

                await _dbContext.Organizers.AddAsync(organizer);
                await _dbContext.SaveChangesAsync();

                var permissions = new HostPermissionsDto
                {
                    CreateEdition = true,
                    EditEdition = true,
                    DeleteEdition = true,
                    CreateQuiz = true,
                    EditQuiz = true,
                    DeleteQuiz = true
                };

                await AddHost(organizer.Id, ownerId, permissions);

                await transaction.CommitAsync();

                return await GetById(organizer.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return null;
            }
        }

        public async Task<HostDto?> AddHost(int organizerId, int userId, HostPermissionsDto permissions)
        {
            var hostOrganizer = new HostOrganizer
            {
                HostId = userId,
                OrganizerId = organizerId,
                CreateEdition = permissions.CreateEdition,
                EditEdition = permissions.EditEdition,
                DeleteEdition = permissions.DeleteEdition,
                CreateQuiz = permissions.CreateQuiz,
                EditQuiz = permissions.EditQuiz,
                DeleteQuiz = permissions.DeleteQuiz
            };

            try
            {
                await _dbContext.HostOrganizers.AddAsync(hostOrganizer);
                await _dbContext.SaveChangesAsync();

                return new()
                {
                    IsOwner = await IsOwner(organizerId, userId),
                    //UserBrief nadopnit kasnije
                    UserBrief = new()
                    {
                        Id = userId,
                    },
                    HostPermissions = permissions
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var organizer = await _dbContext.Organizers.FindAsync(id);

                if (organizer != null)
                {
                    _dbContext.Organizers.Remove(organizer);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteHost(int organizerId, int hostId)
        {
            try
            {
                var host = await _dbContext.HostOrganizers.Where(x => x.OrganizerId == organizerId && x.HostId == hostId).FirstOrDefaultAsync();

                if (host != null)
                {
                    _dbContext.HostOrganizers.Remove(host);
                    await _dbContext.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Organizer?> GetById(int id)
        {
            return await _dbContext.Organizers
                .Include(o => o.Owner)
                .Include(o => o.HostOrganizers)
                    .ThenInclude(h =>  h.Host)
                .Include(o => o.Quizzes)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<HostDto?> GetHost(int organizerId, int hostId)
        {
            var host = await _dbContext.HostOrganizers.Where(x => x.OrganizerId == organizerId && x.HostId == hostId).FirstOrDefaultAsync();

            if (host != null)
            {
                return new()
                {
                    IsOwner = await IsOwner(organizerId, hostId),
                    //UserBrief nadopnit kasnije
                    UserBrief = new()
                    {
                        Id = host.HostId,
                    },
                    HostPermissions = new()
                    {
                        CreateEdition = host.CreateEdition,
                        EditEdition = host.EditEdition,
                        DeleteEdition = host.DeleteEdition,
                        CreateQuiz = host.CreateQuiz,
                        EditQuiz = host.EditQuiz,
                        DeleteQuiz = host.DeleteQuiz
                    }
                };
            }

            return null;
        }

        public async Task<List<HostDto>?> GetHostsFromOrganization(int organizerId)
        {
            var hostOrganizers = await _dbContext.HostOrganizers.Where(x => x.OrganizerId == organizerId).ToListAsync();

            if (hostOrganizers != null)
            {
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
                            CreateQuiz = hostOrganizer.CreateQuiz,
                            EditQuiz = hostOrganizer.EditQuiz,
                            DeleteQuiz = hostOrganizer.DeleteQuiz
                        }
                    };

                    hosts.Add(host);
                }

                return hosts;
            }

            return null;
        }

        public async Task<Organizer?> Update(OrganizerUpdateDto updatedOrganizer)
        {
            var organizer = await _dbContext.Organizers.FindAsync(updatedOrganizer.Id);

            if (organizer != null)
            {
                PropertyUpdater.UpdateEntityFromDto(organizer, updatedOrganizer);

                _dbContext.Entry(organizer).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return await GetById(organizer.Id);
            }

            return null;
        }

        public async Task<HostDto?> UpdateHost(int organizerId, int hostId, HostPermissionsDto permissions)
        {
            var host = await _dbContext.HostOrganizers.Where(x => x.OrganizerId == organizerId && x.HostId == hostId).FirstOrDefaultAsync();

            if (host != null)
            {
                host.CreateEdition = permissions.CreateEdition;
                host.EditEdition = permissions.EditEdition;
                host.DeleteEdition = permissions.DeleteEdition;
                host.CreateQuiz = permissions.CreateQuiz;
                host.EditQuiz = permissions.EditQuiz;
                host.DeleteQuiz = permissions.DeleteQuiz;

                _dbContext.Entry(host).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return new()
                {
                    IsOwner = await IsOwner(organizerId, hostId),
                    //UserBrief nadopnit kasnije
                    UserBrief = new()
                    {
                        Id = host.HostId,
                    },
                    HostPermissions = new()
                    {
                        CreateEdition = host.CreateEdition,
                        EditEdition = host.EditEdition,
                        DeleteEdition = host.DeleteEdition,
                        CreateQuiz = host.CreateQuiz,
                        EditQuiz = host.EditQuiz,
                        DeleteQuiz = host.DeleteQuiz
                    }
                };
            }

            return null;
        }

        private async Task<bool> IsOwner(int organizerId, int hostId) =>
            await _dbContext.Organizers.AnyAsync(x => x.Id == organizerId && x.OwnerId == hostId);
    }
}
