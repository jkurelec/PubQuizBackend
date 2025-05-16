using PubQuizBackend.Model.Dto.OrganizationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IOrganizerService
    {
        public Task<OrganizationBriefDto> Add(string name, int ownerId);
        public Task<HostDto> AddHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions);
        public Task<OrganizationBriefDto> Update(OrganizationUpdateDto updatedOrganizer);
        public Task<HostDto> UpdateHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions);
        public Task<OrganizationBriefDto> GetById(int id);
        public Task<IEnumerable<OrganizationBriefDto>> GetAll();
        public Task<HostDto> GetHost(int organizerId, int hostId, int quizId);
        public Task<IEnumerable<HostDto>> GetHostsFromOrganization(int organizerId);
        public Task<bool> Delete(int id);
        public Task<bool> DeleteHost(int organizerId, int hostId);
        public Task<bool> RemoveHostFromQuiz(int organizerId, int hostId, int quizId);
    }
}
