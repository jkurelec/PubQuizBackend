using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizDto
{
    public class NewQuizDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int OrganizationId { get; set; }
        public List<int> Categories { get; set; } = new();
        public List<int> Locations { get; set; } = new();

        public Quiz ToObject(List<QuizCategory> categories, List<Location> locations)
        {
            return new()
            {
                Name = Name,
                OrganizationId = OrganizationId,
                QuizCategories = categories,
                Locations = locations,
                Rating = 1000
            };
        }
    }
}
