using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizCategoryRepository
    {
        Task<QCategoryDto> GetById(int id);
        Task<List<QCategoryDto>> GetAll();
        Task<QCategoryDto> Add(QCategoryDto quizCategory);
    }
}
