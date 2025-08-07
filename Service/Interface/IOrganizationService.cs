using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Model.Dto.QuizDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IOrganizationService
    {
        Task InviteHostToOrganization(QuizInvitationRequestDto request, int ownerId);
        Task<IEnumerable<QuizInvitationDto>> GetInvitations(int userId);
        Task RespondToInvitation(int userId, ApplicationResponseDto response);
        Task<OrganizationBriefDto> Add(NewOrganizationDto newOraganizer);
        Task<HostDto> AddHost(NewHostDto newHost);
        Task<OrganizationBriefDto> Update(OrganizationUpdateDto updatedOrganizer);
        Task<HostDto> UpdateHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions);
        Task<OrganizationBriefDto> GetById(int id);
        Task<IEnumerable<OrganizationBriefDto>> GetAll();
        Task<HostDto> GetHost(int hostId, int quizId);
        Task<IEnumerable<HostQuizzesDto>> GetHostsFromOrganization(int organizerId);
        Task Delete(int id);
        Task DeleteHost(int organizerId, int hostId);
        Task<string> UpdateProfileImage(int ownerId, IFormFile image);
        Task RemoveHostFromQuiz(int organizerId, int hostId, int quizId);
        Task<IEnumerable<OrganizationMinimalDto>> GetByHost(int hostId);
        Task<OrganizationMinimalDto?> GetOwnerOrganization(int ownerId);
        Task<IEnumerable<QuizMinimalDto>> GetAvaliableQuizzesForNewHost(int hostId, int organizationId);
        Task<IEnumerable<QuizInvitationDto>> GetOrganizationPendingQuizInvitations(int id);
        Task<IEnumerable<HostDto>> GetHostsByQuiz(int quizId);
    }
}
