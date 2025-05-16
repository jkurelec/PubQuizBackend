using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class QuizLeagueDetailedDto
    {
        public QuizLeagueDetailedDto() { }

        public QuizLeagueDetailedDto(QuizLeague league)
        {
            Id = league.Id;
            Name = league.Name;
            Points = LeaguePoints.GetLeaguePointsList(league.Points ?? throw new BadRequestException("Please add points!"));
            Prizes = league.LeaguePrizes.Select(x => new PrizeDto(x)).ToList();
            Quiz = new(league.Quiz);
            Editions = league.QuizEditions.Select(x => new QuizEditionMinimalDto(x)).ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<LeaguePoints> Points { get; set; } = new List<LeaguePoints>();
        public IEnumerable<PrizeDto> Prizes { get; set; } = new List<PrizeDto>();
        public QuizMinimalDto Quiz { get; set; } = null!;
        public IEnumerable<QuizEditionMinimalDto> Editions { get; set; } = new List<QuizEditionMinimalDto>();
        // list trenutacnog stanja 
        //public IEnumerable<TeamMinimalDto> Rankings { get; set; } = new List<QuizEditionMinimalDto>();
    }
}
