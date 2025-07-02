using PubQuizBackend.Model.Dto.ApplicationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizEditionApplicationService
    {
        Task ApplyTeam(QuizEditionApplicationRequestDto application, int registrantId);
        Task WithdrawFromEdition(int editionId, int teamId, int userId);
        Task<IEnumerable<QuizEditionApplicationDto>> GetApplications(int editionId, int hostId, bool unanswered);
        Task RespondToApplication(ApplicationResponseDto applicationDto, int hostId);
        Task RemoveTeamFromEdition(int editionId, int teamId, int userId);
        Task<IEnumerable<AcceptedQuizEditionApplicationDto>> GetAcceptedApplicationsByEdition(int id);
        Task<bool> CheckIfUserApplied(int userId, int editionId);
        Task<bool> CanUserWithdraw(int userId, int teamId, int editionId);
    }
}
