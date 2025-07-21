using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.OrganizationDto;

namespace PubQuizBackend.Model.Dto.QuizDto
{
    public class QuizBriefDto
    {
        public QuizBriefDto(Quiz quiz)
        {
            Id = quiz.Id;
            Name = quiz.Name;
            Organization = new(quiz.Organization);
            Rating = quiz.Rating;
            EditionsHosted = quiz.EditionsHosted;
            ProfileImage = quiz.ProfileImage;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public OrganizationMinimalDto Organization { get; set; } = null!;
        public int Rating { get; set; }
        public int EditionsHosted { get; set; }
        public string? ProfileImage { get; set; }
    }
}
