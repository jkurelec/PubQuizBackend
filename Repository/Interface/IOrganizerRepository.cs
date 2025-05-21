using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.OrganizationDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IOrganizerRepository
    {
        public Task<Organization> Add(string name, int ownerId);
        public Task<HostDto> AddHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions);
        public Task<Organization> Update(OrganizationUpdateDto updatedOrganizer);
        public Task<HostDto> UpdateHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions);
        public Task<Organization> GetById(int id);
        public Task<IEnumerable<Organization>> GetAll();
        public Task<HostDto> GetHost(int organizerId, int hostId, int quizId);
        public Task<IEnumerable<HostDto>> GetHostsFromOrganization(int organizerId);
        public Task<bool> Delete(int id);
        public Task<bool> RemoveHostFromQuiz(int organizerId, int hostId, int quizId);
        public Task<bool> DeleteHost(int organizerId, int hostId);
    }
}
