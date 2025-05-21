using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.TeamDto
{
    public class TeamMemberDto
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public bool RegisterTeam { get; set; }

        public UserTeam ToObject()
        {
            return new()
            {
                UserId = UserId,
                TeamId = TeamId,
                RegisterTeam = RegisterTeam
            };
        }
    }
}
