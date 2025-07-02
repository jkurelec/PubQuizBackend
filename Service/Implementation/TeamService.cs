using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using System;

namespace PubQuizBackend.Service.Implementation
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public async Task<TeamDetailedDto> Add(string name, int ownerId)
        {
            return new(await _teamRepository.Add(name, ownerId));
        }

        public async Task InviteUser(TeamMemberDto teamMember, int ownerId)
        {
            var userExists = await _userRepository.UserExists(teamMember.UserId);

            if (!userExists)
                throw new BadHttpRequestException($"User with id => {teamMember.UserId}!");

            var team = await _teamRepository.GetById(teamMember.TeamId)
                ?? throw new BadRequestException($"No team found with id => {teamMember.TeamId}!");

            if (team.OwnerId != ownerId)
                throw new ForbiddenException();

            var userInTeam = await _teamRepository.UserInTeam(teamMember.UserId, teamMember.TeamId);

            if (userInTeam)
                throw new BadHttpRequestException($"User with id => {teamMember.UserId} is already in team with id => {teamMember.TeamId}!");

            var alreadyInvited = await _teamRepository.AlreadyInvited(teamMember.TeamId, teamMember.UserId);

            if (alreadyInvited)
                throw new BadHttpRequestException($"User with id => {teamMember.UserId} is already invited to team with id => {teamMember.TeamId}!");

            await _teamRepository.InviteUser(teamMember);
        }

        public async Task ChangeOwner(int newOwnerId, int oldOwnerId)
        {
            await _teamRepository.ChangeOwner(newOwnerId, oldOwnerId);
        }

        public async Task Delete(int ownerId)
        {
            await _teamRepository.Delete(ownerId);
        }

        public async Task EditMember(TeamMemberDto teamMember, int ownerId)
        {
            await _teamRepository.EditMember(teamMember, ownerId);
        }

        public async Task<List<TeamBreifDto>> GetAll()
        {
            var teams = await _teamRepository.GetAll();

            return teams.Select(x => new TeamBreifDto(x)).ToList();
        }

        public async Task<TeamDetailedDto> GetById(int id)
        {
            return new(await _teamRepository.GetById(id));
        }

        public async Task<TeamDetailedDto> GetByOwnerId(int id)
        {
            return new(await _teamRepository.GetByOwnerId(id));
        }

        public async Task<IEnumerable<TeamDetailedDto>> GetByUserId(int userId)
        {
            var teams = await _teamRepository.GetByUserId(userId);

            return teams.Select(x => new TeamDetailedDto(x)).ToList();
        }

        public async Task RemoveMember(int userId, int ownerId)
        {
            await _teamRepository.RemoveMember(userId, ownerId);
        }

        public async Task<TeamDetailedDto> Update(UpdateTeamDto teamDto, int ownerId)
        {
            return new(await _teamRepository.Update(teamDto, ownerId));
        }

        public async Task ApplyToTeam(int teamId, int userId)
        {
            _ = await _teamRepository.GetById(teamId);

            var alreadyApplied = await _teamRepository.AlreadyApplied(teamId, userId);

            if (alreadyApplied)
                throw new BadRequestException($"Already applied to team with id => {teamId}!");

            await _teamRepository.ApplyToTeam(teamId, userId);
        }

        public async Task AnswerApplication(int applicationId, int ownerId, bool response)
        {
            var application = await _teamRepository.GetApplication(applicationId);

            if (application.Team.OwnerId != ownerId)
                throw new ForbiddenException();

            await _teamRepository.AnswerApplication(applicationId, response);
        }

        public async Task AnswerInvitation(int invitationId, int userId, bool response)
        {
            await _teamRepository.AnswerInvitation(invitationId, response);
        }

        public async Task<IEnumerable<TeamApplicationInvitationDto>> GetTeamApplications(int ownerId)
        {
            var applications = await _teamRepository.GetTeamApplications(ownerId);

            return applications.Select(x => new TeamApplicationInvitationDto(x)).ToList();
        }

        public async Task<IEnumerable<TeamApplicationInvitationDto>> GetTeamInvitations(int userId)
        {
            var invitations = await _teamRepository.GetTeamInvitations(userId);

            return invitations.Select(x => new TeamApplicationInvitationDto(x)).ToList();
        }

        public async Task LeaveTeam(int userId, int teamId)
        {
            await _teamRepository.LeaveTeam(userId, teamId);
        }

        public async Task<int?> GetIdByOwnerId(int id)
        {
            return await _teamRepository.GetIdByOwnerId(id);
        }

        public async Task<IEnumerable<TeamRegisterDto>> GetTeamsForRegistration(int userId, int editionId)
        {
            var teams = await _teamRepository.GetTeamsForRegistration(userId, editionId);

            return teams.Select(x => new TeamRegisterDto(x)).ToList();
        }
    }
}
