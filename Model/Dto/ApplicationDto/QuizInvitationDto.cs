using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.ApplicationDto
{
    public class QuizInvitationDto
    {
        public QuizInvitationDto() { }

        public QuizInvitationDto(QuizInvitation invitation)
        {
            InvitationId = invitation.Id;
            User = new (invitation.User);
            Quiz = new (invitation.Quiz);
        }

        public int InvitationId { get; set; }
        public UserBriefDto User { get; set; } = null!;
        public QuizMinimalDto Quiz { get; set; } = null!;
    }
}
