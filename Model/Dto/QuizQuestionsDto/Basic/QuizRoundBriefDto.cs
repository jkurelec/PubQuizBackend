using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic
{
    public class QuizRoundBriefDto
    {
        public QuizRoundBriefDto() { }

        public QuizRoundBriefDto(QuizRound round)
        {
            Id = round.Id;
            Number = round.Number;
            Points = round.Points;
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public decimal Points { get; set; }
    }
}
