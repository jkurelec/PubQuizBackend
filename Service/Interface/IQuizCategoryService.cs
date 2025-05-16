using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizCategoryService
    {
        public QCategoryDto GetById(int id);
        public IEnumerable<QCategoryDto> GetByName(string name);
        public IEnumerable<QCategoryDto> GetBySuperCategoryId(int? superCategoryId);
        public IEnumerable<QCategoryDto> GetAll();
        public Task<QCategoryDto> Add(QCategoryDto quizCategory);
    }
}
