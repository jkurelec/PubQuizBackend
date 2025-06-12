using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class NewQuizSegmentResultDto
    {
        public int SegmentId { get; set; }
        public decimal BonusPoints { get; set; }
        public IEnumerable<NewQuizAnswerDto> QuizAnswers { get; set; } = new List<NewQuizAnswerDto>();

        public QuizSegmentResult ToObject()
        {
            return new()
            {
                SegmentId = SegmentId,
                BonusPoints = BonusPoints,
                QuizAnswers = QuizAnswers.Select(x => x.ToObject()).ToList()
            };
        }
    }
}
