using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizRepository
    {
        Task<Quiz> Add(NewQuizDto quizDto, int hostId);
        Task<Quiz> GetById(int id);
        Task<Quiz> GetByIdDetailed(int id);
        Task<IEnumerable<Quiz>> GetAll();
        Task<IEnumerable<Quiz>> GetAllDetailed();
        Task<Quiz> Update(NewQuizDto quizDto, int hostId);
        Task<bool> Delete(int id, int hostId);
    }
}
