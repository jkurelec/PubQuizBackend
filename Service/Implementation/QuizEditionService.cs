using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizEditionService : IQuizEditionService
    {
        private readonly IQuizEditionRepository _repository;

        public QuizEditionService(IQuizEditionRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId)
        {
            return new (await _repository.Add(editionDto, userId));
        }

        public async Task<bool> Delete(int editionId, int userId)
        {
            return await _repository.Delete(editionId, userId);
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetAll()
        {
            var editions = await _repository.GetAll();

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> GetById(int id)
        {
            return new(await _repository.GetById(id));
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id)
        {
            var editions = await _repository.GetByQuizId(id);

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId)
        {
            return new(await _repository.Update(editionDto,userId));
        }
    }
}
