namespace PubQuizBackend.Model.Dto.ApplicationDto
{
    public class QuizEditionApplicationRequestDto
    {
        public int EditionId { get; set; }
        public int TeamId { get; set; }
        public IEnumerable<int> UserIds { get; set; } = new List<int>();
    }
}
