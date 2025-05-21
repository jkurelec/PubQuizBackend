using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizRepository
    {
        public Task<Quiz> Add(NewQuizDto quizDto, int hostId);
        public Task<Quiz> GetById(int id);
        public Task<Quiz> GetByIdDetailed(int id);
        public Task<IEnumerable<Quiz>> GetAll();
        public Task<IEnumerable<Quiz>> GetAllDetailed();
        public Task<Quiz> Update(NewQuizDto quizDto, int hostId);
        public Task<bool> Delete(int id, int hostId);
    }
}
