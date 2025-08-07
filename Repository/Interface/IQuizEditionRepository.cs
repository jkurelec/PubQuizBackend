using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizEditionRepository
    {
        Task<IEnumerable<QuizEdition>> GetAll();
        Task<IEnumerable<QuizEdition>> GetByQuizId(int id);
        Task<QuizEdition> GetById(int id);
        Task<IEnumerable<QuizEdition>> GetPage(int page, int pageSize, EditionFilter editionFilter);
        Task<IEnumerable<QuizEdition>> GetUpcomingCompletedPage(int page, int pageSize, EditionFilter editionFilter, bool upcoming = true);
        Task<int> GetTotalCount(EditionTimeFilter filter);
        Task<QuizEdition> GetByIdDetailed(int id, int? userId = null);
        Task<QuizEdition> Add(NewQuizEditionDto editionDto, int userId);
        Task<QuizEdition> Update(NewQuizEditionDto editionDto, int userId);
        Task Delete(QuizEdition edition);
        Task ApplyTeam(int editionId, int teamId, IEnumerable<int> userIds, int registrantId);
        Task<IEnumerable<QuizEditionApplication>> GetApplications(int editionId, int hostId, bool unanswered);
        Task RespondToApplication(int applicationId, bool applicationResponse, int hostId);
        Task RemoveTeamFromEdition(int editionId, int teamId);
        Task WithdrawFromEdition(int editionId, int userId);
        Task<IEnumerable<QuizEdition>> GetByLocationId(int locationId);
        Task<bool?> HasDetailedQuestions(int editionId);
        Task SetDetailedQuestions(int editionId, int userId, bool detailed);
        Task Save();
    }
}
