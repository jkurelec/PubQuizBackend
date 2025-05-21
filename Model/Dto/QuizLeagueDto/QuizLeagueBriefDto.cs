using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Model.Dto.QuizLeagueDto
{
    public class QuizLeagueBriefDto
    {
        public QuizLeagueBriefDto() { }

        public QuizLeagueBriefDto(QuizLeague league)
        {
            Id = league.Id;
            Name = league.Name;
            Quiz = new(league.Quiz);
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public QuizMinimalDto Quiz { get; set; } = null!;
    }
}
