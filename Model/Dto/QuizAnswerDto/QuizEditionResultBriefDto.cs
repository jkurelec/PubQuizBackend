using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizEditionResultBriefDto
    {
        public QuizEditionResultBriefDto(QuizEditionResult editionResult)
        {
            Id = editionResult.Id;
            Team = new (editionResult.Team);
            Rounds = editionResult.QuizRoundResults.Select(x => new QuizRoundResultMinimalDto(x));
            TotalPoints = editionResult.TotalPoints;
            Rank = editionResult.Rank ?? 0;
        }

        public int Id { get; set; }
        public TeamBreifDto Team { get; set; } = null!;
        public IEnumerable<QuizRoundResultMinimalDto> Rounds { get; set; } = new List<QuizRoundResultMinimalDto>();
        public decimal TotalPoints { get; set; }
        public int Rank { get; set; }
    }
}
