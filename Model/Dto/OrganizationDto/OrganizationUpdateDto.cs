namespace PubQuizBackend.Model.Dto.OrganizationDto
{
    public class OrganizationUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int OwnerId { get; set; }
    }
}
