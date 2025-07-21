using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizDto
{
    public class QuizMinimalDto
    {
        public QuizMinimalDto(Quiz quiz)
        {
            Id = quiz.Id;
            Name = quiz.Name;
            Rating = quiz.Rating;
            EditionsHosted = quiz.EditionsHosted;
            ProfileImage = quiz.ProfileImage;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rating { get; set; }
        public int EditionsHosted { get; set; }
        public string? ProfileImage { get; set; }
    }
}
