using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class NewQuizRoundResultDto
    {
        public int RoundId { get; set; }
        public int EditionResultId { get; set; }
        public decimal Points { get; set; }
        public IEnumerable<NewQuizSegmentResultDto> QuizSegmentResults { get; set; } = new List<NewQuizSegmentResultDto>();

        public QuizRoundResult ToObject()
        {
            return new()
            {
                RoundId = RoundId,
                EditionResultId = EditionResultId,
                Points = Points,
                QuizSegmentResults = QuizSegmentResults.Select(x => x.ToObject()).ToList()
            };
        }
    }
}
