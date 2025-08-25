using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizSegmentResultDetailedDto : NewQuizSegmentResultDto
    {
        public QuizSegmentResultDetailedDto() { }

        public QuizSegmentResultDetailedDto(QuizSegmentResult quizSegmentResult)
        {
            Id = quizSegmentResult.Id;
            SegmentId = quizSegmentResult.SegmentId;
            BonusPoints = quizSegmentResult.BonusPoints;
            RoundResultId = quizSegmentResult.RoundResultId;
            QuizAnswers = quizSegmentResult.QuizAnswers.Select(x => new QuizAnswerDetailedDto(x)).ToList();
        }

        public int Id { get; set; }
        public int RoundResultId { get; set; }
        public new IEnumerable<QuizAnswerDetailedDto> QuizAnswers { get; set; } = new List<QuizAnswerDetailedDto>();

        public new QuizSegmentResult ToObject()
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
