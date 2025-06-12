using PubQuizBackend.Model.Dto.OrganizationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IOrganizationService
    {
        Task<OrganizationBriefDto> Add(NewOrganizationDto newOraganizer);
        Task<HostDto> AddHost(NewHostDto newHost);
        Task<OrganizationBriefDto> Update(OrganizationUpdateDto updatedOrganizer);
        Task<HostDto> UpdateHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions);
        Task<OrganizationBriefDto> GetById(int id);
        Task<IEnumerable<OrganizationBriefDto>> GetAll();
        Task<HostDto> GetHost(int organizerId, int hostId, int quizId);
        Task<IEnumerable<HostDto>> GetHostsFromOrganization(int organizerId);
        Task Delete(int id);
        Task DeleteHost(int organizerId, int hostId);
        Task RemoveHostFromQuiz(int organizerId, int hostId, int quizId);
    }
}
