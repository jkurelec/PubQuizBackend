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
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly PubQuizContext _context;
        private readonly MediaServerClient _mediaServerClient;

        public OrganizationRepository(PubQuizContext context, MediaServerClient mediaServerClient)
        {
            _context = context;
            _mediaServerClient = mediaServerClient;
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
                OwnerId = ownerId,
                ProfileImage = "default.jpg"
            };

            var organizerEntityEntry = await _context.Organizations.AddAsync(organizer);

            var owner = await _context.Users.FindAsync(ownerId)
                ?? throw new NotFoundException("User not found!");

            if (owner.Role == (int)Role.ATTENDEE)
                owner.Role = (int)Role.ORGANIZER;

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

            var user = await _context.Users.FindAsync(hostId)
                ?? throw new NotFoundException("User not found!");

            if (user.Role == (int)Role.ATTENDEE)
                user.Role = (int)Role.ORGANIZER;

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
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var host = await _context.HostOrganizationQuizzes
                    .Where(x => x.OrganizationId == organizationId && x.HostId == hostId)
                    .ToListAsync();

                if (host == null || host.Count == 0)
                    throw new ConflictException("Who are you deleting?!?");

                _context.HostOrganizationQuizzes.RemoveRange(host);

                await _context.SaveChangesAsync();

                if (!await IsUserHost(hostId))
                {
                    var user = await _context.Users.FindAsync(hostId)
                        ?? throw new NotFoundException("User not found!");

                    if (user.Role == (int)Role.ORGANIZER)
                        user.Role = (int)Role.ATTENDEE;

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new DivineException();
            }
        }


        public async Task RemoveHostFromQuiz(int organizationId, int hostId, int quizId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var host = await _context.HostOrganizationQuizzes
                    .Where(x => x.OrganizationId == organizationId && x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                        ?? throw new ConflictException("Who are you deleting?!?");

                _context.HostOrganizationQuizzes.Remove(host);
                await _context.SaveChangesAsync();

                if (!await IsUserHost(hostId))
                {
                    var user = await _context.Users.FindAsync(hostId)
                        ?? throw new NotFoundException("User not found!");

                    if (user.Role == (int)Role.ORGANIZER)
                        user.Role = (int)Role.ATTENDEE;

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new DivineException();
            }
        }

        public async Task<Organization> GetById(int id)
        {
            return await _context.Organizations
                .Include(x => x.Owner)
                .Include(x => x.Quizzes)
                .FirstOrDefaultAsync(x => x.Id == id)
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
        public async Task<HostDto> GetHostDto(int hostId, int quizId)
        {
            var host = await _context.HostOrganizationQuizzes
                .Where(x => x.HostId == hostId && x.QuizId == quizId).FirstOrDefaultAsync()
                    ?? throw new NotFoundException("Host not found!");

            return new()
            {
                IsOwner = await IsOwner(host.OrganizationId, hostId),
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

        public async Task<IEnumerable<HostQuizzesDto>> GetHostsFromOrganization(int organizationId)
        {
            var hostOrganizationQuizzes = await _context.HostOrganizationQuizzes
                .Include(hq => hq.Host)
                .Include(hq => hq.Quiz)
                .Where(hq => hq.OrganizationId == organizationId)
                .ToListAsync()
                ?? throw new TotalnoSiToPromislioException();

            var groupedByHost = hostOrganizationQuizzes
                .GroupBy(hq => hq.HostId);

            var result = new List<HostQuizzesDto>();

            foreach (var group in groupedByHost)
            {
                var firstEntry = group.First();
                var isOwner = await IsOwner(organizationId, firstEntry.HostId);

                var hostDto = new HostQuizzesDto
                {
                    IsOwner = isOwner,
                    UserBrief = new UserBriefDto
                    {
                        Id = firstEntry.Host.Id,
                        Username = firstEntry.Host.Username,
                        ProfileImage = firstEntry.Host.ProfileImage,
                        Email = firstEntry.Host.Email,
                        Rating = firstEntry.Host.Rating
                    },
                    QuizPermissions = group.Select(q => new QuizPermissionDto
                    {
                        QuizId = q.QuizId,
                        QuizName = q.Quiz.Name,
                        Permissions = new HostPermissionsDto
                        {
                            CreateEdition = q.CreateEdition,
                            EditEdition = q.EditEdition,
                            DeleteEdition = q.DeleteEdition,
                            CrudQuestion = q.CrudQuestion,
                            ManageApplication = q.ManageApplication
                        }
                    }).ToList()
                };

                result.Add(hostDto);
            }

            return result;
        }


        public async Task<Organization> Update(OrganizationUpdateDto updatedOrganization)
        {
            var organizer = await _context.Organizations.FindAsync(updatedOrganization.Id)
                ?? throw new NotFoundException("Organizer not found!");

            if (organizer.OwnerId != updatedOrganization.OwnerId)
            {
                var alreadyOwner = await _context.Organizations.AnyAsync(x => x.OwnerId == updatedOrganization.OwnerId);

                if (alreadyOwner)
                    throw new BadRequestException("User is already an owner of an organization!");
            }

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

        public async Task<bool> IsUserHost(int userId)
        {
            return await _context.HostOrganizationQuizzes.AnyAsync(x => x.HostId == userId);
        }

        public async Task InviteHostToOrganization(int userId, int quizId, int ownerId)
        {
            var isOwnerOfQuiz = await _context.Quizzes
                .AnyAsync(x => x.Id == quizId && x.Organization.OwnerId == ownerId);

            if (!isOwnerOfQuiz)
                throw new ForbiddenException();

            var userAlreadyInQuiz = await _context.HostOrganizationQuizzes
                .AnyAsync(x => x.HostId == userId && x.QuizId == quizId);

            if (userAlreadyInQuiz)
                throw new BadRequestException("User is already a part of this quiz!");

            var userAlreadyInvitedToQuiz = await _context.QuizInvitations
                .AnyAsync(x => x.UserId == userId && x.QuizId == quizId);

            if (userAlreadyInvitedToQuiz)
                throw new BadRequestException("User is already invited to this quiz!");

            await _context.QuizInvitations.AddAsync(
                new()
                {
                    UserId = userId,
                    QuizId = quizId,
                    Response = null,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await _context.SaveChangesAsync();
        }

        public async Task RespondToInvitation(int userId, int invitationId, bool accept)
        {
            var invitation = await _context.QuizInvitations
                .Include(x => x.Quiz)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == invitationId && x.Response == null)
                ?? throw new NotFoundException("Invitation not found!");

            if (accept)
                await AddHost(invitation.Quiz.OrganizationId, userId, invitation.QuizId, new HostPermissionsDto
                {
                    CreateEdition = false,
                    EditEdition = false,
                    DeleteEdition = false,
                    CrudQuestion = false,
                    ManageApplication = false
                });

            _context.Remove(invitation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizInvitation>> GetInvitations(int userId)
        {
            return await _context.QuizInvitations
                .Include(x => x.Quiz)
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetByHost(int hostId)
        {
            return await _context.HostOrganizationQuizzes
                .Where(h => h.HostId == hostId)
                .Select(h => h.Organization)
                .Distinct()
                .ToListAsync();
        }

        public async Task<string> UpdateProfileImage(Organization organization, IFormFile image)
        {
            var fileName = await _mediaServerClient.PostImage("/private/update/organization", image, $"{organization.Id}{Path.GetExtension(image.FileName)}");

            if (organization.ProfileImage != fileName)
            {
                organization.ProfileImage = fileName;

                await _context.SaveChangesAsync();
            }

            return fileName;
        }

        public async Task<Organization?> GetOwnerOrganization(int ownerId)
        {
            return await _context.Organizations.FirstOrDefaultAsync(x => x.OwnerId == ownerId);
        }

        private async Task<bool> IsOwner(int organizationId, int userId) =>
            await _context.Organizations.AnyAsync(x => x.Id == organizationId && x.OwnerId == userId);

        public async Task<IEnumerable<Quiz>> GetAvaliableQuizzesForNewHost(int hostId, int organizationId)
        {
            var quizzes = await _context.HostOrganizationQuizzes
                .Include(x => x.Quiz)
                .Where(x =>
                    x.OrganizationId == organizationId &&
                    x.HostId != hostId
                )
                .Select(x => x.Quiz)
                .ToListAsync();

            var alreadyInvitedToQuiz = true;
            var finalQuizzes = new List<Quiz>();

            foreach (var quiz in quizzes)
            {
                alreadyInvitedToQuiz = await _context.QuizInvitations
                    .AnyAsync(x => x.UserId == hostId && x.QuizId == quiz.Id);

                if (!alreadyInvitedToQuiz)
                    finalQuizzes.Add(quiz);
            }

            return finalQuizzes;
        }

        public async Task<IEnumerable<QuizInvitation>> GetOrganizationPendingQuizInvitations(int id)
        {
            return await _context.QuizInvitations
                .Include(x => x.Quiz)
                .Include(x => x.User)
                .Where(x => x.Quiz.OrganizationId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<HostDto>> GetHostsByQuiz(int quizId)
        {
            var hosts = await _context.HostOrganizationQuizzes
                .Include(x => x.Host)
                .Where(x => x.QuizId == quizId)
                .Select(
                    x => new HostDto
                    {
                        IsOwner = false,
                        UserBrief = new UserBriefDto
                        {
                            Id = x.Host.Id,
                            Username = x.Host.Username,
                            Email = x.Host.Email,
                            Rating = x.Host.Rating,
                            ProfileImage = x.Host.ProfileImage
                        },
                        HostPermissions = new HostPermissionsDto
                        {
                            CreateEdition = x.CreateEdition,
                            EditEdition = x.EditEdition,
                            DeleteEdition = x.DeleteEdition,
                            CrudQuestion = x.CrudQuestion,
                            ManageApplication = x.ManageApplication
                        }
                    }
                )
                .ToListAsync();

            var quiz = await _context.Quizzes
                .Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.Id == quizId)
                ?? throw new NotFoundException($"Quiz with id => {quizId} not found!");

            var ownerHost = hosts.FirstOrDefault(x => x.UserBrief.Id == quiz.Organization.OwnerId);

            if (ownerHost != null)
                ownerHost.IsOwner = true;

            return hosts;
        }

        private async Task FillUserInfo(UserBriefDto userBriefDto)
        {
            var hostInfo = await _context.Users.FindAsync(userBriefDto.Id)
                ?? throw new NotFoundException($"User {userBriefDto.Id} not found!");

            userBriefDto.Username = hostInfo.Username;
            userBriefDto.Email = hostInfo.Email;
            userBriefDto.Rating = hostInfo.Rating;
            userBriefDto.ProfileImage = hostInfo.ProfileImage;
        }
    }
}
