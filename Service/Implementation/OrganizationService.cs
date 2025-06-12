using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizerRepository;
        private readonly IUserRepository _userRepository;

        public OrganizationService(IOrganizationRepository organizerRepository, IUserRepository userRepository)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
        }

        public async Task<OrganizationBriefDto> Add(NewOrganizationDto newOraganizer)
        {
            var nameInUse = await _organizerRepository.IsNameInUse(newOraganizer.Name);

            if(nameInUse)
                throw new ConflictException("Name already in use!");

            var userOwner = await _organizerRepository.IsUserOwner(newOraganizer.OwnerId);

            if (userOwner)
                throw new BadRequestException($"User {newOraganizer.OwnerId} is already owner");
            
            return new (await _organizerRepository.Add(newOraganizer.Name, newOraganizer.OwnerId));
        }

        public async Task<HostDto> AddHost(NewHostDto newHostDto)
        {
            var user = await _userRepository.GetById(newHostDto.HostId);

            if (!Enum.TryParse<Role>(user.Role.ToString(), ignoreCase: true, out var role) || role < Role.ORGANIZER)
                throw new BadRequestException($"User {newHostDto.HostId} is not an organizer!");

            var hostInQuiz = await _organizerRepository.IsHostInQuiz(
                newHostDto.OrganizerId,
                newHostDto.HostId,
                newHostDto.QuizId
            );

            if (hostInQuiz)
                throw new BadRequestException($"Host {newHostDto.HostId} is already assigned to the quiz!");

            return await FillHostInfo(
                await _organizerRepository.AddHost(newHostDto.OrganizerId, newHostDto.HostId, newHostDto.QuizId, newHostDto.HostPermissions)
                );
        }

        public async Task Delete(int id)
        {
            var organization = await _organizerRepository.GetById(id);

            await _organizerRepository.Delete(organization);
        }

        public async Task DeleteHost(int organizerId, int hostId)
        {
            await _organizerRepository.DeleteHost(organizerId, hostId);
        }

        public async Task RemoveHostFromQuiz(int organizerId, int hostId, int quizId)
        {
            await _organizerRepository.RemoveHostFromQuiz(organizerId, hostId, quizId);
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
                await _organizerRepository.GetHostDto(organizerId, hostId, quizId)
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
