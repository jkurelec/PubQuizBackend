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
                    QuizId = 1
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

        public async Task AddMember(TeamMemberDto teamMember, int ownerId)
        {
            var team = await _context.Teams.FindAsync(teamMember.TeamId)
                ?? throw new BadRequestException("No team!");

            if (team.OwnerId != ownerId)
                throw new ForbiddenException();

            var exists = await _context.UserTeams.FindAsync(teamMember.UserId, teamMember.TeamId);

            if (exists != null)
                throw new BadRequestException("User already in team!");

            await _context.UserTeams.AddAsync(teamMember.ToObject());
            await _context.SaveChangesAsync();
        }

        public async Task ChangeOwner(int newOwnerId, int oldOwnerId)
        {
            var team = await _context.Teams.Include(x => x.UserTeams).FirstOrDefaultAsync(x => x.OwnerId ==  oldOwnerId)
                ?? throw new UnauthorizedAccessException();

            var newOwner = await _context.Users.FindAsync(newOwnerId)
                ?? throw new BadRequestException("New owner not found!");

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
                .Where(t => t.UserTeams.Any(ut => ut.UserId == userId))
                .Include(t => t.Category)
                .Include(t => t.Quiz)
                .ToListAsync();
        }
    }
}
