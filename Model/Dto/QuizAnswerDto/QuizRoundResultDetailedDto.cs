using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizRoundResultDetailedDto : NewQuizRoundResultDto
    {
        public QuizRoundResultDetailedDto() { }

        public QuizRoundResultDetailedDto(QuizRoundResult roundResult)
        {
            Id = roundResult.Id;
            RoundId = roundResult.RoundId;
            EditionResultId = roundResult.EditionResultId;
            Points = roundResult.Points;
            QuizSegmentResults = roundResult.QuizSegmentResults.Select(x => new QuizSegmentResultDetailedDto(x)).ToList();
        }

        public int Id { get; set; }
        public new IEnumerable<QuizSegmentResultDetailedDto> QuizSegmentResults { get; set; } = new List<QuizSegmentResultDetailedDto>();
    }
}
