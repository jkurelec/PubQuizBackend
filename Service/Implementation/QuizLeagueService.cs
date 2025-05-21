using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizLeagueDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizLeagueService : IQuizLeagueService
    {
        private readonly IQuizLeagueRepository _repository;

        public QuizLeagueService(IQuizLeagueRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuizLeagueDetailedDto> Add(NewQuizLeagueDto leagueDto, int userId)
        {
            return new(await _repository.Add(leagueDto, userId));
        }

        public async Task<bool> Delete(int id, int userId)
        {
            return await _repository.Delete(id, userId);
        }

        public async Task<QuizLeagueDetailedDto> GetById(int id)
        {
            return new(await _repository.GetById(id));
        }

        public async Task<IEnumerable<QuizLeagueBriefDto>> GetByQuizId(int id)
        {
            var quizzes = await _repository.GetByQuizId(id);

            return quizzes.Select(x => new QuizLeagueBriefDto(x)).ToList();
        }

        public async Task<QuizLeagueDetailedDto> Update(NewQuizLeagueDto leagueDto, int userId)
        {
            return new(await _repository.Update(leagueDto, userId));
        }
    }
}
