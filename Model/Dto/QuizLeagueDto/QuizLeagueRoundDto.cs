using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class QuizLeagueRoundDto
    {
        public QuizLeagueRoundDto() { }

        public QuizLeagueRoundDto(QuizLeagueRound round)
        {
            Id = round.Id;
            Round = round.Round;
            QuizLeagueRoundEntries = round.QuizLeagueEntries.Select(x => new QuizLeagueRoundEntryDto(x)).ToList();
        }

        public int Id { get; set; }
        public int Round { get; set; }
        public IEnumerable<QuizLeagueRoundEntryDto> QuizLeagueRoundEntries { get; set; } = new List<QuizLeagueRoundEntryDto>();
    }
}
