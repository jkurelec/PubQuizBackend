using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.OrganizerDto;
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

        public async Task<OrganizerDto?> Add(string name, int ownerId)
        {
            var organizer = await _organizerRepository.Add(name, ownerId);

            if (organizer != null)
                return new(organizer);

            return null;
        }

        public async Task<HostDto?> AddHost(int organizerId, int hostId, HostPermissionsDto permissions)
        {
            var host = await _organizerRepository.AddHost(organizerId, hostId, permissions);

            return await FillHostInfo(host);
        }

        public async Task<bool> Delete(int id)
        {
            return await _organizerRepository.Delete(id);
        }

        public async Task<bool> DeleteHost(int organizerId, int hostId)
        {
            return await _organizerRepository.DeleteHost(organizerId, hostId);
        }

        public async Task<OrganizerDto?> GetById(int id)
        {
            var organizer = await _organizerRepository.GetById(id);

            if (organizer != null)
                return new(organizer);

            return null;
        }

        public async Task<HostDto?> GetHost(int organizerId, int hostId)
        {
            var host = await _organizerRepository.GetHost(organizerId, hostId);

            return await FillHostInfo(host);
        }

        public async Task<List<HostDto>?> GetHostsFromOrganization(int organizerId)
        {
            var hosts = await _organizerRepository.GetHostsFromOrganization(organizerId);

            if (hosts != null)
            {
                foreach (var host in hosts)
                {
                    await FillHostInfo(host);
                }
            }

            return hosts;
        }

        public async Task<OrganizerDto?> Update(OrganizerUpdateDto updatedOrganizer)
        {
            var organizer = await _organizerRepository.Update(updatedOrganizer);

            if (organizer != null)
                return new(organizer);

            return null;
        }

        public async Task<HostDto?> UpdateHost(int organizerId, int hostId, HostPermissionsDto permissions)
        {
            var host = await _organizerRepository.UpdateHost(organizerId, hostId, permissions);

            return await FillHostInfo(host);
        }

        private async Task<HostDto?> FillHostInfo(HostDto? hostDto)
        {

            if (hostDto != null)
            {
                var hostInfo = await _userRepository.GetById(hostDto.UserBrief.Id);

                if (hostInfo != null)
                {
                    hostDto.UserBrief.Username = hostInfo.Username;
                    hostDto.UserBrief.Email = hostInfo.Email;
                    hostDto.UserBrief.Rating = hostInfo.Rating;

                    return hostDto;
                }
            }

            return null;
        }
    }
}
