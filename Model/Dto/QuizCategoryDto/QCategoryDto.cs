using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizCategoryDto
{
    public class QCategoryDto
    {
        public QCategoryDto() {}

        public QCategoryDto(QuizCategory quizCategory)
        {
            Id = quizCategory.Id;
            Name = quizCategory.Name;
            SuperCategoryId = quizCategory.SuperCategoryId;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? SuperCategoryId { get; set; }

        public QuizCategory ToObject()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                SuperCategoryId = SuperCategoryId
            };
        }
    }
}
