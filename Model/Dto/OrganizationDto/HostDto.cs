using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class HostDto
    {
        public HostDto() { }

        public HostDto(bool isOwner, UserBriefDto userBrief, HostPermissionsDto hostPermissions)
        {
            IsOwner = isOwner;
            UserBrief = userBrief;
            HostPermissions = hostPermissions;
        }

        public bool IsOwner { get; set; } = false;
        public required UserBriefDto UserBrief { get; set; }
        public required HostPermissionsDto HostPermissions { get; set; }
    }
}
