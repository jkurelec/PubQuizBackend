using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Service.Interface
{
    public interface ITeamService
    {
        public Task AddMember(TeamMemberDto teamMember, int ownerId);

        public Task ChangeOwner(int newOwnerId, int oldOwnerId);

        public Task Delete(int ownerId);

        public Task EditMember(TeamMemberDto teamMember, int ownerId);

        public Task RemoveMember(int userId, int ownerId);

        public Task<List<TeamBreifDto>> GetAll();

        public Task<TeamDetailedDto> Add(string name, int ownerId);

        public Task<TeamDetailedDto> GetById(int id);

        public Task<TeamDetailedDto> GetByOwnerId(int id);

        public Task<TeamDetailedDto> Update(UpdateTeamDto teamDto, int ownerId);
    }
}