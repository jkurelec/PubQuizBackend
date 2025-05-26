using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic
{
    public class QuizRoundDto
    {
        public QuizRoundDto() { }

        public QuizRoundDto(QuizRound round)
        {
            Id = round.Id;
            Number = round.Number;
            EditionId = round.EditionId;
            QuizSegments = round.QuizSegments.Select(x => new QuizSegmentDto(x)).ToList();
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public int EditionId { get; set; }
        public IEnumerable<QuizSegmentDto> QuizSegments { get; set; } = new List<QuizSegmentDto>();

        public QuizRound ToObject()
        {
            return new()
            {
                Id = Id,
                Number = Number,
                EditionId = EditionId,
            };
        }
    }
}
