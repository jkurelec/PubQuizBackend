using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _repository;

        public QuizService(IQuizRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuizDetailedDto> Add(NewQuizDto quizDto, int hostId)
        {
            return new(await _repository.Add(quizDto, hostId));
        }

        public async Task<bool> Delete(int id, int hostId)
        {
            return await _repository.Delete(id, hostId);
        }

        public async Task<IEnumerable<object>> GetAll(bool detailed = false)
        {
            return detailed
                ? await _repository.GetAllDetailed().ContinueWith(x => x.Result.Select(x => new QuizDetailedDto(x)))
                : await _repository.GetAll().ContinueWith(x => x.Result.Select(x => new QuizBriefDto(x)));
        }

        public async Task<object> GetById(int id, bool detailed = false)
        {
            return detailed
                ? await _repository.GetByIdDetailed(id).ContinueWith(x => new QuizDetailedDto(x.Result))
                : await _repository.GetById(id).ContinueWith(x => new QuizBriefDto(x.Result));
        }

        public async Task<QuizDetailedDto> Update(NewQuizDto quizDto, int hostId)
        {
            return new(await _repository.Update(quizDto, hostId));
        }
    }
}
