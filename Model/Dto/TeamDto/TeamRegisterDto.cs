using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.TeamDto
{
    public class TeamRegisterDto
    {
        public TeamRegisterDto() { }

        public TeamRegisterDto(Team team)
        {
            Id = team.Id;
            Name = team.Name;
            Memebers = team.UserTeams.Select(x => new UserBriefDto(x.User));
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IEnumerable<UserBriefDto> Memebers { get; set; } = new List<UserBriefDto>();
    }
}
