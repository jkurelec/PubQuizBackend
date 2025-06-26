using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repository;

        public TeamService(ITeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<TeamDetailedDto> Add(string name, int ownerId)
        {
            return new(await _repository.Add(name, ownerId));
        }

        public async Task AddMember(TeamMemberDto teamMember, int ownerId)
        {
            await _repository.AddMember(teamMember, ownerId);
        }

        public async Task ChangeOwner(int newOwnerId, int oldOwnerId)
        {
            await _repository.ChangeOwner(newOwnerId, oldOwnerId);
        }

        public async Task Delete(int ownerId)
        {
            await _repository.Delete(ownerId);
        }

        public async Task EditMember(TeamMemberDto teamMember, int ownerId)
        {
            await _repository.EditMember(teamMember, ownerId);
        }

        public async Task<List<TeamBreifDto>> GetAll()
        {
            var teams = await _repository.GetAll();

            return teams.Select(x => new TeamBreifDto(x)).ToList();
        }

        public async Task<TeamDetailedDto> GetById(int id)
        {
            return new(await _repository.GetById(id));
        }

        public async Task<TeamDetailedDto> GetByOwnerId(int id)
        {
            return new(await _repository.GetByOwnerId(id));
        }

        public async Task<IEnumerable<TeamDetailedDto>> GetByUserId(int userId)
        {
            var teams = await _repository.GetByUserId(userId);

            return teams.Select(x => new TeamDetailedDto(x)).ToList();
        }

        public async Task RemoveMember(int userId, int ownerId)
        {
            await _repository.RemoveMember(userId, ownerId);
        }

        public async Task<TeamDetailedDto> Update(UpdateTeamDto teamDto, int ownerId)
        {
            return new(await _repository.Update(teamDto, ownerId));
        }
    }
}
