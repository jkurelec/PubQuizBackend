using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class QuizLeagueMinimalDto
    {
        public QuizLeagueMinimalDto() { }

        public QuizLeagueMinimalDto(QuizLeague league)
        {
            Id = league.Id;
            Name = league.Name;
            Finished = league.Finished;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Finished { get; set; }
    }
}
