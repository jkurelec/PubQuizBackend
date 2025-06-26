using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.ApplicationDto
{
    public class AcceptedQuizEditionApplicationDto
    {
        public AcceptedQuizEditionApplicationDto(QuizEditionApplication application)
        {
            Id = application.Id;
            TeamId = application.Team.Id;
            TeamName = application.Team.Name;
            TeamMembers = application.Users.Count;
        }

        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public int TeamMembers{ get; set; }
    }
}
