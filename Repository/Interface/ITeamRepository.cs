using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface ITeamRepository
    {
        Task AddMember(TeamMemberDto teamMember, int ownerId);
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
        Task<IEnumerable<Team>> GetByUserId(int id);
        Task<Team> Update(UpdateTeamDto teamDto, int ownerId);
    }
}
