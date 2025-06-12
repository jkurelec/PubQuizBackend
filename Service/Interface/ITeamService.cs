using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Service.Interface
{
    public interface ITeamService
    {
        Task AddMember(TeamMemberDto teamMember, int ownerId);
        Task ChangeOwner(int newOwnerId, int oldOwnerId);
        Task Delete(int ownerId);
        Task EditMember(TeamMemberDto teamMember, int ownerId);
        Task RemoveMember(int userId, int ownerId);
        Task<List<TeamBreifDto>> GetAll();
        Task<TeamDetailedDto> Add(string name, int ownerId);
        Task<TeamDetailedDto> GetById(int id);
        Task<TeamDetailedDto> GetByOwnerId(int id);
        Task<TeamDetailedDto> Update(UpdateTeamDto teamDto, int ownerId);
    }
}