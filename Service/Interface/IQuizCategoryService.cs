using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizCategoryService
    {
        QCategoryDto GetById(int id);
        IEnumerable<QCategoryDto> GetByName(string name);
        IEnumerable<QCategoryDto> GetBySuperCategoryId(int? superCategoryId);
        IEnumerable<QCategoryDto> GetAll();
        Task<QCategoryDto> Add(QCategoryDto quizCategory);
    }
}
