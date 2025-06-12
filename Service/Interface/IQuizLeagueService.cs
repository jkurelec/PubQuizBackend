using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizLeagueDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizLeagueService
    {
        Task<QuizLeagueDetailedDto> Add(NewQuizLeagueDto leagueDto, int userId);
        Task<bool> Delete(int id, int userId);
        Task<QuizLeagueDetailedDto> GetById(int id);
        Task<IEnumerable<QuizLeagueBriefDto>> GetByQuizId(int id);
        Task<QuizLeagueDetailedDto> Update(NewQuizLeagueDto leagueDto, int userId);
    }
}
