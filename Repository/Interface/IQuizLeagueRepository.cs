using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizLeagueDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizLeagueRepository
    {
        public Task<QuizLeague> Add(NewQuizLeagueDto leagueDto, int userId);
        public Task<bool> Delete(int id, int userId);
        public Task<QuizLeague> GetById(int id);
        public Task<IEnumerable<QuizLeague>> GetByQuizId(int id);
        public Task<QuizLeague> Update(NewQuizLeagueDto leagueDto, int userId);
    }
}
