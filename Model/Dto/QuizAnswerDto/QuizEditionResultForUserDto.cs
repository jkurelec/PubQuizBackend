using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizEditionResultForUserDto
    {
        public QuizEditionResultForUserDto() { }

        public QuizEditionResultForUserDto(QuizEditionResult quizEditionResult)
        {
            Id = quizEditionResult.Id;
            Rank = quizEditionResult.Rank;
            TeamPoints = quizEditionResult.TotalPoints;
            TotalPoints = quizEditionResult.Edition.TotalPoints;
            Rating = quizEditionResult.Rating;
            Edition = new (quizEditionResult.Edition);
            Team = new (quizEditionResult.Team);
        }

        public int Id { get; set; }
        public int? Rank { get; set; }
        public decimal TeamPoints { get; set; }
        public decimal TotalPoints { get; set; }
        public int Rating { get; set; }
        public virtual QuizEditionMinimalDto Edition { get; set; } = null!;
        public virtual TeamBreifDto Team { get; set; } = null!;
    }
}
