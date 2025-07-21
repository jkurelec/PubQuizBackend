using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class TeamRepository : ITeamRepository
    {
        private readonly  PubQuizContext _context;

        public TeamRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<Team> Add(string name, int ownerId)
        {
            // MOGU LI TIMOVI IMATI ISTO IME ? 
            //var exists = await _context.Teams.AnyAsync(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant());

            //if (exists)
            //    throw new BadRequestException("Name already taken!");

            var owner = await _context.Users.FirstOrDefaultAsync(x => x.Id == ownerId)
                ?? throw new TotalnoSiToPromislioException();

            var hasTeam = await _context.Teams.AnyAsync(x => x.OwnerId == ownerId);

            if (hasTeam)
                throw new BadRequestException("Already has a team!");

            var entity = await _context.Teams.AddAsync(
                new Team
                {
                    Name = name,
                    OwnerId = owner.Id,
                    CategoryId = 1,
                    QuizId = 1,
                    ProfileImage = "default.jpg"
                }
            );

            await _context.SaveChangesAsync();

            await _context.UserTeams.AddAsync(
                new()
                {
                    UserId = ownerId,
                    TeamId = entity.Entity.Id,
                    RegisterTeam = true
                }
            );

            await _context.SaveChangesAsync();

            return await GetById(entity.Entity.Id);
        }

        public async Task InviteUser(TeamMemberDto teamMember)
        {
            await _context.TeamInvitations.AddAsync(
                new TeamInvitation
                {
                    UserId = teamMember.UserId,
                    TeamId = teamMember.TeamId,
                    CreatedAt = DateTime.UtcNow,
                    Response = null
                }
            );

            await _context.SaveChangesAsync();
        }

        public async Task ChangeOwner(int newOwnerId, int oldOwnerId)
        {
            var team = await _context.Teams.Include(x => x.UserTeams).FirstOrDefaultAsync(x => x.OwnerId ==  oldOwnerId)
                ?? throw new BadRequestException();

            var newOwner = await _context.Users.FindAsync(newOwnerId)
                ?? throw new BadRequestException("New owner not found!");

            var alreadyOwner = await _context.Teams.AnyAsync(x => x.OwnerId == newOwnerId);

            if (alreadyOwner)
                throw new BadRequestException($"User with id => {newOwnerId} is already an owner of a team!");

            if (!team.UserTeams.Any(x => x.UserId == newOwnerId))
                throw new ForbiddenException();

            team.OwnerId = newOwnerId;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int ownerId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.OwnerId == ownerId)
                ?? throw new ForbiddenException();

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();
        }

        public async Task EditMember(TeamMemberDto teamMember, int ownerId)
        {
            var team = await _context.Teams.FindAsync(teamMember.TeamId)
                ?? throw new BadRequestException("No team!");

            if (team.OwnerId != ownerId)
                throw new ForbiddenException();

            var member = await _context.UserTeams.FindAsync(teamMember.UserId, teamMember.TeamId)
                ?? throw new BadRequestException("Member not found!");

            if (member.RegisterTeam == teamMember.RegisterTeam)
                throw new BadRequestException("Did not change anything!");

            member.RegisterTeam = teamMember.RegisterTeam;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Team>> GetAll()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team> GetById(int id)
        {
            return await _context.Teams
                .Include(x => x.Category)
                .Include(x => x.Quiz)
                .Include(x => x.UserTeams)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("No team found!");
        }

        public async Task<UserTeam> GetMemeberById(int userId, int teamId)
        {
            return await _context.UserTeams.FindAsync(userId, teamId)
                ?? throw new NotFoundException("No member found!");
        }

        public async Task<IEnumerable<int>> FilterMemberIdsInTeam(IEnumerable<int> userIds, int teamId)
        {
            return await _context.UserTeams
                .Where(x => x.TeamId == teamId && userIds.Contains(x.UserId))
                .Select(x => x.UserId)
                .ToListAsync();
        }

        public async Task<Team> GetByOwnerId(int id)
        {
            return await _context.Teams
                .Include(x => x.Category)
                .Include(x => x.Quiz)
                .Include(x => x.UserTeams)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(x => x.OwnerId == id)
                    ?? throw new NotFoundException("No team found!");
        }

        public async Task RemoveMember(int userId, int ownerId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.OwnerId == ownerId)
                ?? throw new BadRequestException("No team!");

            var member = await _context.UserTeams.FindAsync(userId, team.Id)
                ?? throw new BadRequestException("Member not found!");

            _context.UserTeams.Remove(member);
            await _context.SaveChangesAsync();
        }

        public async Task<Team> Update(UpdateTeamDto teamDto, int ownerId)
        {
            var team = await _context.Teams
                .Include(x => x.Category)
                .Include(x => x.Quiz)
                .FirstOrDefaultAsync(x => x.OwnerId == ownerId)
                    ?? throw new ForbiddenException();

            if (team.OwnerId != ownerId)
                throw new ForbiddenException();

            team.Name = teamDto.Name;

            await _context.SaveChangesAsync();

            return team;
        }

        public async Task<IEnumerable<Team>> GetByUserId(int userId)
        {
            return await _context.Teams
                .Where(x => x.UserTeams.Any(ut => ut.UserId == userId))
                .Include(x => x.UserTeams)
                    .ThenInclude(u => u.User)
                .Include(x => x.Category)
                .Include(x => x.Quiz)
                .ToListAsync();
        }

        public async Task<bool> UserInTeam(int userId, int teamId)
        {
            return await _context.UserTeams.AnyAsync(x => x.UserId == userId && x.TeamId == teamId);
        }

        public async Task ApplyToTeam(int teamId, int userId)
        {
            await _context.TeamApplications.AddAsync(
                new TeamApplication
                {
                    UserId = userId,
                    TeamId = teamId,
                    CreatedAt = DateTime.UtcNow,
                    Response = null
                }
            );

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<TeamApplication>> GetTeamApplications(int ownerId)
        {
            return await _context.TeamApplications
                .Include(x => x.User)
                .Where(x => x.Team.OwnerId == ownerId && x.Response == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeamInvitation>> GetTeamInvitations(int userId)
        {
            return await _context.TeamInvitations
                .Include(x => x.Team)
                .Where(x => x.UserId == userId && x.Response == null)
                .ToListAsync();
        }

        public async Task AnswerApplication(int applicationId, bool response)
        {
            var application = await _context.TeamApplications.FindAsync(applicationId)
                ?? throw new NotFoundException("Application not found!");

            application.Response = response;

            if (response)
            {
                var userTeam = new UserTeam
                {
                    UserId = application.UserId,
                    TeamId = application.TeamId,
                    RegisterTeam = false
                };

                await _context.UserTeams.AddAsync(userTeam);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AnswerInvitation(int invitationId, bool response)
        {
            var invitation = await _context.TeamInvitations.FindAsync(invitationId)
                ?? throw new NotFoundException("Invitation not found!");

            invitation.Response = response;

            if (response)
            {
                var userTeam = new UserTeam
                {
                    UserId = invitation.UserId,
                    TeamId = invitation.TeamId,
                    RegisterTeam = false
                };

                await _context.UserTeams.AddAsync(userTeam);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AlreadyInvited(int teamId, int userId)
        {
            return await _context.TeamInvitations
                .AnyAsync(x => x.TeamId == teamId && x.UserId == userId && x.Response == null);
        }

        public async Task<bool> AlreadyApplied(int teamId, int userId)
        {
            return await _context.TeamApplications
                .AnyAsync(x => x.TeamId == teamId && x.UserId == userId && x.Response == null);
        }

        public async Task<TeamApplication> GetApplication(int applicationId)
        {
            return await _context.TeamApplications
                .Include(x => x.Team)
                .FirstOrDefaultAsync(x => x.Id == applicationId)
                ?? throw new NotFoundException("Application not found!");
        }

        public async Task<TeamInvitation> GetInvitation(int invitationId)
        {
            return await _context.TeamInvitations
                .FirstOrDefaultAsync(x => x.Id == invitationId)
                ?? throw new NotFoundException("Invitation not found!");
        }

        public async Task LeaveTeam(int userId, int teamId)
        {
            var user = await _context.UserTeams
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TeamId == teamId)
                ?? throw new NotFoundException("User not in team!");

            _context.UserTeams.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetIdByOwnerId(int id)
        {
            return await _context.Teams
                .Where(x => x.OwnerId == id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsForRegistration(int userId, int editionId)
        {
            var teams = await _context.Teams
                .AsNoTracking()
                .Include(x => x.UserTeams)
                    .ThenInclude(ut => ut.User)
                        .ThenInclude(u => u.Applications)
                .Where(x => x.UserTeams.Any(ut => ut.UserId == userId && ut.RegisterTeam) && !x.QuizEditionApplications.Any(x => x.EditionId == editionId))
                .ToListAsync();

            foreach (var team in teams)
            {
                foreach(var userTeam in team.UserTeams.ToList())
                {
                    if (userTeam.User.Applications.Any(x => x.EditionId == editionId))
                        team.UserTeams.Remove(userTeam);
                }
            }

            return teams;
        }
    }
}
