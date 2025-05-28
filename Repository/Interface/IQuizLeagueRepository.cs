using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizLeagueDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizLeagueRepository
    {
        Task<QuizLeague> Add(NewQuizLeagueDto leagueDto, int userId);
        Task<bool> Delete(int id, int userId);
        Task<QuizLeague> GetById(int id);
        Task<QuizLeague> GetByIdDetailed(int id);
        Task<IEnumerable<QuizLeague>> GetByQuizId(int id);
        Task<QuizLeague> Update(NewQuizLeagueDto leagueDto, int userId);
    }
}
