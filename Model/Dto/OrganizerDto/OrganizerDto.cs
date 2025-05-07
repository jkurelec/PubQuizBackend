using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.OrganizerDto
{
    public class OrganizerDto
    {
        public OrganizerDto() {}

        public OrganizerDto(Organizer organizer)
        {
            Id = organizer.Id;
            Name = organizer.Name;
            EditionsHosted = organizer.EditionsHosted;
            Owner = new(organizer.Owner);
            Hosts = organizer.HostOrganizers.Select(x => new UserBriefDto(x.Host)).ToList();
            Quizzes = organizer.Quizzes.ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int EditionsHosted { get; set; }
        public UserBriefDto Owner { get; set; } = null!;
        public List<UserBriefDto> Hosts { get; set; } = null!;
        public List<Quiz> Quizzes { get; set; } = null!;
    }
}
