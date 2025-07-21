using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class OrganizationMinimalDto
    {
        public OrganizationMinimalDto() { }

        public OrganizationMinimalDto(Organization organization)
        {
            Id = organization.Id;
            Name = organization.Name;
            ProfileImage = organization.ProfileImage;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ProfileImage { get; set; }
    }
}
