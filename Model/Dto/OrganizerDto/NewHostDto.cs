namespace PubQuizBackend.Model.Dto.OrganizerDto
{
    public class NewHostDto
    {
        public int OrganizerId { get; set; }
        public int HostId { get; set; }
        public HostPermissionsDto HostPermissions { get; set; } = null!;
    }
}
