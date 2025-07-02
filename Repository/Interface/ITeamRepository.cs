using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface ITeamRepository
    {
        Task InviteUser(TeamMemberDto teamMember);
        Task ChangeOwner(int newOwnerId, int oldOwnerId);
        Task Delete(int ownerId);
        Task EditMember(TeamMemberDto teamMember, int ownerId);
        Task RemoveMember(int userId, int ownerId);
        Task<IEnumerable<int>> FilterMemberIdsInTeam(IEnumerable<int> userIds, int teamId);
        Task<List<Team>> GetAll();
        Task<Team> Add(string name, int ownerId);
        Task<UserTeam> GetMemeberById(int userId, int teamId);
        Task<Team> GetById(int id);
        Task<Team> GetByOwnerId(int id);
        Task<int?> GetIdByOwnerId(int id);
        Task<IEnumerable<Team>> GetByUserId(int id);
        Task<Team> Update(UpdateTeamDto teamDto, int ownerId);
        Task<bool> UserInTeam(int userId, int teamId);
        Task ApplyToTeam(int teamId, int userId);
        Task AnswerApplication(int applicationId, bool response);
        Task AnswerInvitation(int invitationId, bool response);
        Task<IEnumerable<TeamApplication>> GetTeamApplications(int ownerId);
        Task<IEnumerable<TeamInvitation>> GetTeamInvitations(int userId);
        Task<bool> AlreadyInvited(int teamId, int userId);
        Task<bool> AlreadyApplied(int teamId, int userId);
        Task<TeamApplication> GetApplication(int applicationId);
        Task<TeamInvitation> GetInvitation(int invitationId);
        Task LeaveTeam(int userId, int teamId);
        Task<IEnumerable<Team>> GetTeamsForRegistration(int userId, int editionId);
    }
}
