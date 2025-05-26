using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic
{
    public class QuizSegmentDto
    {
        public QuizSegmentDto() { }

        public QuizSegmentDto(QuizSegment segment)
        {
            Id = segment.Id;
            RoundId = segment.RoundId;
            BonusPoints = segment.BonusPoints;
            Number = segment.Number;
            Type = Enum.IsDefined(typeof(SegmentType), segment.Type)
                ? (SegmentType)segment.Type
                : throw new BadRequestException("Invalid segment type!");
            Questions = segment.QuizQuestions.Select(x => new QuizQuestionDto(x)).ToList();
        }

        public int Id { get; set; }
        public int RoundId { get; set; }
        public decimal BonusPoints { get; set; } = 0;
        public int Number { get; set; }
        public SegmentType Type { get; set; } = SegmentType.REGULAR;
        public IEnumerable<QuizQuestionDto> Questions { get; set; } = new List<QuizQuestionDto>();

        public QuizSegment ToObject()
        {
            return new()
            {
                Id = Id,
                RoundId = RoundId,
                BonusPoints = BonusPoints,
                Number = Number,
                Type = Enum.IsDefined(Type)
                    ? (int)Type
                    : throw new BadRequestException("Invalid segment type!")
            };
        }
    }
}
