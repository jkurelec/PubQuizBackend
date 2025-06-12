using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizAnswerDetailedDto : NewQuizAnswerDto
    {
        public QuizAnswerDetailedDto() { }

        public QuizAnswerDetailedDto(QuizAnswer quizAnswer)
        {
            Id = quizAnswer.Id;
            Answer = quizAnswer.Answer;
            Points = quizAnswer.Points;
            QuestionId = quizAnswer.QuestionId;
            Result = quizAnswer.Result;
            SegmentResultId = quizAnswer.SegmentResultId;
        }

        public int Id { get; set; }
        public int SegmentResultId { get; set; }
    }
}
