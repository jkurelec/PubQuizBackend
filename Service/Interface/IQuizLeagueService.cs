using PubQuizBackend.Model.Dto.QuizLeagueDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizLeagueService
    {
        Task<QuizLeagueDetailedDto> Add(NewQuizLeagueDto leagueDto, int userId);
        Task<bool> Delete(int id, int userId);
        Task<QuizLeagueBriefDto> GetBriefById(int id);
        Task<QuizLeagueDetailedDto> GetDetailedById(int id);
        Task<IEnumerable<QuizLeagueBriefDto>> GetByQuizId(int id);
        Task<QuizLeagueDetailedDto> Update(NewQuizLeagueDto leagueDto, int userId);
        Task<QuizLeagueDetailedDto> FinishLeague(int leagueId, int userId);
    }
}
