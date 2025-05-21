using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.TeamDto
{
    public class TeamBreifDto
    {
        public TeamBreifDto() { }

        public TeamBreifDto(Team team)
        {
            Id = team.Id;
            Name = team.Name;
            Rating = team.Rating;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rating { get; set; }
    }
}
