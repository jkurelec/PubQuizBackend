using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizRoundResultMinimalDto
    {
        public QuizRoundResultMinimalDto(QuizRoundResult roundResult)
        {
            Id = roundResult.Id;
            RoundId = roundResult.RoundId;
            EditionResultId = roundResult.EditionResultId;
            Points = roundResult.Points;
        }

        public int Id { get; set; }
        public int RoundId { get; set; }
        public int EditionResultId { get; set; }
        public decimal Points { get; set; }
    }
}
