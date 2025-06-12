namespace PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int? SuperId { get; set; } = null;
    }
}
