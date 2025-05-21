using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizCategoryRepository
    {
        public Task<QCategoryDto> GetById(int id);
        public Task<List<QCategoryDto>> GetAll();
        public Task<QCategoryDto> Add(QCategoryDto quizCategory);
    }
}
