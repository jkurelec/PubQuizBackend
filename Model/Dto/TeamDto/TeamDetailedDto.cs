using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.TeamDto
{
    public class TeamDetailedDto
    {
        public TeamDetailedDto() { }

        public TeamDetailedDto(Team team)
        {
            Id = team.Id;
            Name = team.Name;
            Rating = team.Rating;
            Category = new(team.Category);
            Quiz = new(team.Quiz);
            TeamMembers = team.UserTeams.Select(x => new UserBriefDto(x.User)).ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rating { get; set; }
        public QCategoryDto Category { get; set; } = null!;
        public QuizMinimalDto Quiz { get; set; } = null!;
        public IEnumerable<UserBriefDto> TeamMembers { get; set; } = new List<UserBriefDto>();
    }
}
