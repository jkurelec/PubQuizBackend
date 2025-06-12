using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class NewQuizAnswerDto
    {
        public string Answer { get; set; } = null!;
        public decimal Points { get; set; }
        public int QuestionId { get; set; }
        public int Result { get; set; }

        public QuizAnswer ToObject()
        {
            return new()
            {
                Answer = Answer,
                Points = Points,
                QuestionId = QuestionId,
                Result = Result,
            };
        }
    }
}
