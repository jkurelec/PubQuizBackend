using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IUserRepository _userRepository;

        public OrganizerService(IOrganizerRepository organizerRepository, IUserRepository userRepository)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
        }

        public async Task<OrganizationBriefDto> Add(string name, int ownerId)
        {
            return new(
                await _organizerRepository.Add(name, ownerId)
            );
        }

        public async Task<HostDto> AddHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions)
        {
            return await FillHostInfo(
                await _organizerRepository.AddHost(organizerId, hostId, quizId, permissions)
                );
        }

        public async Task<bool> Delete(int id)
        {
            return await _organizerRepository.Delete(id);
        }

        public async Task<bool> DeleteHost(int organizerId, int hostId)
        {
            return await _organizerRepository.DeleteHost(organizerId, hostId);
        }

        public async Task<bool> RemoveHostFromQuiz(int organizerId, int hostId, int quizId)
        {
            return await _organizerRepository.RemoveHostFromQuiz(organizerId, hostId, quizId);
        }

        public async Task<OrganizationBriefDto> GetById(int id)
        {
            return new(await _organizerRepository.GetById(id));
        }

        public async Task<IEnumerable<OrganizationBriefDto>> GetAll()
        {
            var organizers = await _organizerRepository.GetAll();
            return organizers.Select(x => new OrganizationBriefDto(x));
        }

        public async Task<HostDto> GetHost(int organizerId, int hostId, int quizId)
        {
            return await FillHostInfo(
                await _organizerRepository.GetHost(organizerId, hostId, quizId)
            );
        }

        public async Task<IEnumerable<HostDto>> GetHostsFromOrganization(int organizerId)
        {
            var hosts = await _organizerRepository.GetHostsFromOrganization(organizerId);

            foreach (var host in hosts)
                await FillHostInfo(host);

            return hosts;
        }

        public async Task<OrganizationBriefDto> Update(OrganizationUpdateDto updatedOrganizer)
        {
            return new(
                await _organizerRepository.Update(updatedOrganizer)
            );
        }

        public async Task<HostDto> UpdateHost(int organizerId, int hostId, int quizId, HostPermissionsDto permissions)
        {
            return await FillHostInfo(
                await _organizerRepository.UpdateHost(organizerId, hostId, quizId, permissions)
            );
        }

        private async Task<HostDto> FillHostInfo(HostDto hostDto)
        {
            var hostInfo = await _userRepository.GetById(hostDto.UserBrief.Id);

            hostDto.UserBrief.Username = hostInfo.Username;
            hostDto.UserBrief.Email = hostInfo.Email;
            hostDto.UserBrief.Rating = hostInfo.Rating;

            return hostDto;
        }
    }
}
