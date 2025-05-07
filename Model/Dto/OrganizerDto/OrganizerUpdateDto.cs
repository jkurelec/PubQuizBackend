namespace PubQuizBackend.Model.Dto.OrganizerDto
{
    public class OrganizerUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int OwnerId { get; set; }
    }
}
