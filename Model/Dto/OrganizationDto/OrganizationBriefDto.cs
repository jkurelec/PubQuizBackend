using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class OrganizationBriefDto
    {
        public OrganizationBriefDto() { }

        public OrganizationBriefDto(Organization organizer)
        {
            Id = organizer.Id;
            Name = organizer.Name;
            EditionsHosted = organizer.EditionsHosted;
            Owner = new(organizer.Owner);
            Quizzes = organizer.Quizzes.Select(x => new QuizMinimalDto(x)).ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int EditionsHosted { get; set; }
        public UserBriefDto Owner { get; set; } = null!;
        public List<QuizMinimalDto> Quizzes { get; set; } = null!;
    }
}
