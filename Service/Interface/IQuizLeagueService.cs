using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizLeagueDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizLeagueService
    {
        public Task<QuizLeagueDetailedDto> Add(NewQuizLeagueDto leagueDto, int userId);
        public Task<bool> Delete(int id, int userId);
        public Task<QuizLeagueDetailedDto> GetById(int id);
        public Task<IEnumerable<QuizLeagueBriefDto>> GetByQuizId(int id);
        public Task<QuizLeagueDetailedDto> Update(NewQuizLeagueDto leagueDto, int userId);
    }
}
