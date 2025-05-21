using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface ITeamRepository
    {
        public Task<Team> Add(string name, int ownerId);
        public Task<Team> Update(UpdateTeamDto teamDto, int ownerId);
        public Task Delete(int ownerId);
        public Task<Team> GetById(int id);
        public Task<Team> GetByOwnerId(int id);
        public Task<List<Team>> GetAll();
        public Task ChangeOwner(int newOwnerId, int oldOwnerId);
        public Task AddMember(TeamMemberDto teamMember, int ownerId);
        public Task EditMember(TeamMemberDto teamMember, int ownerId);
        public Task RemoveMember(int userId, int ownerId);
    }
}
