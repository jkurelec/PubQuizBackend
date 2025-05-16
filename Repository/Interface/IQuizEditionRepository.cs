using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizEditionRepository
    {
        public Task<IEnumerable<QuizEdition>> GetAll();
        public Task<IEnumerable<QuizEdition>> GetByQuizId(int id);
        public Task<QuizEdition> GetById(int id);
        public Task<QuizEdition> Add(NewQuizEditionDto editionDto, int userId);
        public Task<QuizEdition> Update(NewQuizEditionDto editionDto, int userId);
        public Task<bool> Delete(int editionId, int userId);
    }
}
