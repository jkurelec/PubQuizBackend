using PubQuizBackend.Model.Dto.OrganizerDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IOrganizerService
    {
        public Task<OrganizerDto?> Add(string name, int ownerId);
        public Task<HostDto?> AddHost(int organizerId, int hostId, HostPermissionsDto permissions);
        public Task<OrganizerDto?> Update(OrganizerUpdateDto updatedOrganizer);
        public Task<HostDto?> UpdateHost(int organizerId, int hostId, HostPermissionsDto permissions);
        public Task<OrganizerDto?> GetById(int id);
        public Task<HostDto?> GetHost(int organizerId, int hostId);
        public Task<List<HostDto>?> GetHostsFromOrganization(int organizerId);
        public Task<bool> Delete(int id);
        public Task<bool> DeleteHost(int organizerId, int hostId);
    }
}
