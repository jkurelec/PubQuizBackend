using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class QuizLeagueRoundEntryDto
    {
        public QuizLeagueRoundEntryDto() { }

        public QuizLeagueRoundEntryDto(QuizLeagueEntry entry)
        {
            Id = entry.Id;
            Points = entry.Points;
            Team = new (entry.Team);
        }

        public int Id { get; set; }
        public double Points { get; set; }
        public TeamBreifDto Team { get; set; } = null!;
    }
}
