using PubQuizBackend.Model.Dto.QuizDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizService
    {
        public Task<QuizDetailedDto> Add(NewQuizDto quizDto, int hostId);
        public Task<object> GetById(int id, bool detailed = false);
        public Task<IEnumerable<object>> GetAll(bool detailed = false);
        public Task<QuizDetailedDto> Update(NewQuizDto quizDto, int hostId);
        public Task<bool> Delete(int id, int hostId);
    }
}
