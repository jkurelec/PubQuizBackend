namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class HostPermissionsDto
    {
        public bool CreateEdition { get; set; } = false;
        public bool EditEdition { get; set; } = false;
        public bool DeleteEdition { get; set; } = false;
        public bool CrudQuestion { get; set; } = false;
    }
}
