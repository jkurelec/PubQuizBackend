using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.OrganizationDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IOrganizationRepository
    {
        Task InviteHostToOrganization(int userId, int quizId, int ownerId);
        Task<IEnumerable<QuizInvitation>> GetInvitations(int userId);
        Task RespondToInvitation(int userId, int invitationId, bool accept);
        Task Delete(Organization organization);
        Task DeleteHost(int organizationId, int hostId);
        Task<bool> IsNameInUse(string name);
        Task<bool> IsUserOwner(int userId);
        Task<bool> IsHostInQuiz(int organizationId, int hostId, int quizId);
        Task<bool> DoesHostExist(int hostId, int quizId);
        Task RemoveHostFromQuiz(int organizationId, int hostId, int quizId);
        Task<HostDto> AddHost(int organizationId, int hostId, int quizId, HostPermissionsDto permissions);
        Task<HostOrganizationQuiz> GetHostByEditionId(int hostId, int editionId);
        Task<HostOrganizationQuiz> GetHost(int hostId, int quizId);
        Task<HostDto> GetHostDto(int organizationId, int hostId, int quizId);
        Task<HostDto> UpdateHost(int organizationId, int hostId, int quizId, HostPermissionsDto permissions);
        Task<IEnumerable<HostQuizzesDto>> GetHostsFromOrganization(int organizationId);
        Task<IEnumerable<Organization>> GetAll();
        Task<Organization> Add(string name, int ownerId);
        Task<Organization> GetById(int id);
        Task<Organization> GetByIdForBriefDto(int id);
        Task<Organization> GetByIdForDetailedDto(int id);
        Task<Organization> Update(OrganizationUpdateDto updatedOrganization);
        Task<bool> IsUserHost(int userId);
        Task<IEnumerable<Organization>> GetByHost(int hostId);
        Task<Organization?> GetOwnerOrganization(int ownerId);
        Task<string> UpdateProfileImage(Organization organization, IFormFile image);
        Task<IEnumerable<Quiz>> GetAvaliableQuizzesForNewHost(int hostId, int organizationId);
        Task<IEnumerable<QuizInvitation>> GetOrganizationPendingQuizInvitations(int id);
        Task<IEnumerable<HostDto>> GetHostsByQuiz(int quizId);
    }
}
