using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizCategoryService : IQuizCategoryService
    {
        private readonly IQuizCategoryRepository _repository;

        public QuizCategoryService(IQuizCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<QCategoryDto> Add(QCategoryDto quizCategory)
        {
            return await _repository.Add(quizCategory);
        }

        public IEnumerable<QCategoryDto> GetAll()
        {
            return QuizCategoryProvider.GetAll();
        }

        public QCategoryDto GetById(int id)
        {
            return QuizCategoryProvider.GetById(id);
        }

        public IEnumerable<QCategoryDto> GetByName(string name)
        {
            return QuizCategoryProvider.GetBySimilarName(name);
        }

        public IEnumerable<QCategoryDto> GetBySuperCategoryId(int? superCategoryId)
        {
            return QuizCategoryProvider.GetBySuperCategoryId(superCategoryId);
        }
    }
}
