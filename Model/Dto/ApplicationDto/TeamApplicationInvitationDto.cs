using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.ApplicationDto
{
    public class TeamApplicationInvitationDto
    {
        public TeamApplicationInvitationDto() { }

        public TeamApplicationInvitationDto(TeamApplication application)
        {
            Id = application.Id;
            Name = application.User.Username;
            UserId = application.UserId;
            TeamId = application.TeamId;
            Response = application.Response;
            CreatedAt = application.CreatedAt;
        }

        public TeamApplicationInvitationDto(TeamInvitation invitation)
        {
            Id = invitation.Id;
            Name = invitation.Team.Name;
            UserId = invitation.UserId;
            TeamId = invitation.TeamId;
            Response = invitation.Response;
            CreatedAt = invitation.CreatedAt;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public bool? Response { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
