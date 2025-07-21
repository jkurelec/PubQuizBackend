using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class HostQuizzesDto
    {
        public HostQuizzesDto() { }

        public HostQuizzesDto(bool isOwner, UserBriefDto userBrief, List<QuizPermissionDto> quizPermissions)
        {
            IsOwner = isOwner;
            UserBrief = userBrief;
            QuizPermissions = quizPermissions;
        }

        public bool IsOwner { get; set; } = false;
        public required UserBriefDto UserBrief { get; set; }
        public required List<QuizPermissionDto> QuizPermissions { get; set; }
    }

    public class QuizPermissionDto
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public HostPermissionsDto Permissions { get; set; } = new();
    }
}
