using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class OrganizationDetailedDto
    {
        public OrganizationDetailedDto() { }

        public OrganizationDetailedDto(Organization organizer)
        {
            Id = organizer.Id;
            Name = organizer.Name;
            EditionsHosted = organizer.EditionsHosted;
            Owner = new(organizer.Owner);
            Hosts = organizer.HostOrganizationQuizzes.Select(x => new UserBriefDto(x.Host)).ToList();
            Quizzes = organizer.Quizzes.Select(x => new QuizMinimalDto(x)).ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int EditionsHosted { get; set; }
        public UserBriefDto Owner { get; set; } = null!;
        public List<UserBriefDto> Hosts { get; set; } = null!;
        public List<QuizMinimalDto> Quizzes { get; set; } = null!;
    }
}
