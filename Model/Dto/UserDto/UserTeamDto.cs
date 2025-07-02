using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.UserDto
{
    public class UserTeamDto : UserBriefDto
    {
        public UserTeamDto() { }

        public UserTeamDto(UserTeam userTeam) : base(userTeam.User)
        {
            RegisterTeam = userTeam.RegisterTeam;
        }

        public bool RegisterTeam { get; set; } = false;
    }
}
