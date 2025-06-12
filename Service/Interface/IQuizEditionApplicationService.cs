using PubQuizBackend.Model.Dto.ApplicationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizEditionApplicationService
    {
        Task ApplyTeam(QuizEditionApplicationRequestDto application, int registrantId);
        Task WithdrawFromEdition(int editionId, int teamId, int userId);
        Task<IEnumerable<QuizEditionApplicationDto>> GetApplications(int editionId, int hostId, bool unanswered);
        Task RespondToApplication(QuizEditionApplicationResponseDto application, int hostId);
        Task RemoveTeamFromEdition(int editionId, int teamId, int userId);
    }
}
