using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific
{
    public class QuizQuestionDetailedDto
    {
        public int Id { get; set; }
        public int SegmentId { get; set; }
        public int Type { get; set; }
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
        public decimal Points { get; set; }
        public decimal BonusPoints { get; set; }
        public string? MediaUrl { get; set; }
        public int Number { get; set; }
        public virtual IEnumerable<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
        public virtual QuizSegment Segment { get; set; } = null!;
    }
}
