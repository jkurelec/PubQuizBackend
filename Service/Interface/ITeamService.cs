using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Service.Interface
{
    public interface ITeamService
    {
        Task InviteUser(TeamMemberDto teamMember, int ownerId);
        Task ChangeOwner(int newOwnerId, int oldOwnerId);
        Task Delete(int ownerId);
        Task EditMember(TeamMemberDto teamMember, int ownerId);
        Task RemoveMember(int userId, int ownerId);
        Task<List<TeamBreifDto>> GetAll();
        Task<TeamDetailedDto> Add(string name, int ownerId);
        Task<TeamDetailedDto> GetById(int id);
        Task<TeamDetailedDto> GetByOwnerId(int id);
        Task<TeamDetailedDto> Update(UpdateTeamDto teamDto, int ownerId);
        Task<IEnumerable<TeamDetailedDto>> GetByUserId(int userId);
        Task ApplyToTeam(int teamId, int userId);
        Task AnswerApplication(int applicationId, int ownerId, bool response);
        Task AnswerInvitation(int invitationId, int userId, bool response);
        Task<IEnumerable<TeamApplicationInvitationDto>> GetTeamApplications(int ownerId);
        Task<IEnumerable<TeamApplicationInvitationDto>> GetTeamInvitations(int userId);
        Task LeaveTeam(int userId, int teamId);
        Task<int?> GetIdByOwnerId(int id);
        Task<IEnumerable<TeamRegisterDto>> GetTeamsForRegistration(int userId, int editionId);
        Task<bool> CanInviteUser(int inviter, int invitee);
    }
}