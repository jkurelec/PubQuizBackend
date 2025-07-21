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
            ProfileImage = team.ProfileImage;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ProfileImage { get; set; }
    }
}
