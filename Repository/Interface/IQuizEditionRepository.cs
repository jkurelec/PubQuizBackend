using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizEditionRepository
    {
        Task<IEnumerable<QuizEdition>> GetAll();
        Task<IEnumerable<QuizEdition>> GetByQuizId(int id);
        Task<QuizEdition> GetById(int id);
        Task<QuizEdition> GetByIdDetailed(int id);
        Task<QuizEdition> Add(NewQuizEditionDto editionDto, int userId);
        Task<QuizEdition> Update(NewQuizEditionDto editionDto, int userId);
        Task Delete(QuizEdition edition);
        Task ApplyTeam(int editionId, int teamId, IEnumerable<int> userIds, int registrantId);
        Task<IEnumerable<QuizEditionApplication>> GetApplications(int editionId, int hostId, bool unanswered);
        Task RespondToApplication(int applicationId, bool applicationResponse, int hostId);
        Task RemoveTeamFromEdition(int editionId, int teamId);
        Task WithdrawFromEdition(int editionId, int teamId);
    }
}
