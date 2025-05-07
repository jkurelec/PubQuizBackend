using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizCategoryRepository
    {
        public Task<QuizCategoryDto?> GetById(int id);
        public Task<List<QuizCategoryDto>> GetAll();
        public Task<QuizCategoryDto?> Add(QuizCategoryDto quizCategory);
    }
}
