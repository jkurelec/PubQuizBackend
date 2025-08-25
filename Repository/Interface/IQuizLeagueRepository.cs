using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizLeagueDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizLeagueRepository
    {
        Task<QuizLeague> Add(NewQuizLeagueDto leagueDto, int userId);
        Task<bool> Delete(int id, int userId);
        Task<QuizLeague> GetBriefById(int id);
        Task<QuizLeague> GetByIdDetailed(int id);
        Task<IEnumerable<QuizLeague>> GetByQuizId(int id);
        Task<QuizLeague> Update(NewQuizLeagueDto leagueDto, int userId);
        Task<QuizLeague> FinishLeague(int leagueId, int userId);
        Task AddLeagueRound(int leagueId, IEnumerable<(int, int)> entries);
    }
}
