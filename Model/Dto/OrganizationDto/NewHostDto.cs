namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class NewHostDto
    {
        public int OrganizerId { get; set; }
        public int HostId { get; set; }
        public int QuizId { get; set; }
        public HostPermissionsDto HostPermissions { get; set; } = null!;
    }
}
