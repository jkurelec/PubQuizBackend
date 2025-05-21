using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class NewQuizLeagueDto
    {
        public NewQuizLeagueDto()
        {
        }

        public NewQuizLeagueDto(QuizLeague league)
        {
            Id = league.Id;
            Name = league.Name;
            QuizId = league.QuizId;
            Points = league.Points!;
            Prizes = league.LeaguePrizes.Select(x => new PrizeDto(x));
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int QuizId { get; set; }
        public string Points { get; set; } = null!;
        public IEnumerable<PrizeDto> Prizes { get; set; } = new List<PrizeDto>();

        public QuizLeague ToObject()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                QuizId = QuizId,
                Points = Points,
            };
        }
    }
}
