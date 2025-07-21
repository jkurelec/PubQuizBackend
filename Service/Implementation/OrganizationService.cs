using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuizRepository _quizRepository;

        public OrganizationService(IOrganizationRepository organizerRepository, IUserRepository userRepository, IQuizRepository quizRepository)
        {
            _organizerRepository = organizerRepository;
            _userRepository = userRepository;
            _quizRepository = quizRepository;
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

        public async Task<IEnumerable<HostQuizzesDto>> GetHostsFromOrganization(int organizerId)
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
            hostDto.UserBrief.ProfileImage = hostInfo.ProfileImage;

            return hostDto;
        }

        private async Task<HostQuizzesDto> FillHostInfo(HostQuizzesDto hostDto)
        {
            var hostInfo = await _userRepository.GetById(hostDto.UserBrief.Id);

            hostDto.UserBrief.Username = hostInfo.Username;
            hostDto.UserBrief.Email = hostInfo.Email;
            hostDto.UserBrief.Rating = hostInfo.Rating;
            hostDto.UserBrief.ProfileImage = hostInfo.ProfileImage;

            return hostDto;
        }

        public async Task InviteHostToOrganization(QuizInvitationRequestDto request, int ownerId)
        {
            await _organizerRepository.InviteHostToOrganization(
                request.UserId,
                request.QuizId,
                ownerId
            );
        }

        public async Task<IEnumerable<QuizInvitationDto>> GetInvitations(int userId)
        {
            var invitations = await _organizerRepository.GetInvitations(userId);

            return invitations.Select(x => new QuizInvitationDto(x)).ToList();
        }

        public async Task RespondToInvitation(int userId, ApplicationResponseDto response)
        {
            await _organizerRepository.RespondToInvitation(
                userId,
                response.ApplicationId,
                response.Response
            );
        }

        public async Task<IEnumerable<OrganizationMinimalDto>> GetByHost(int hostId)
        {
            var organizations = await _organizerRepository.GetByHost(hostId);

            return organizations.Select(x => new OrganizationMinimalDto(x)).ToList();
        }

        public async Task<OrganizationMinimalDto?> GetOwnerOrganization(int ownerId)
        {
            var organization = await _organizerRepository.GetOwnerOrganization(ownerId);

            if (organization != null)
                return new (organization);

            return null;
        }

        public async Task<string> UpdateProfileImage(int ownerId, IFormFile image)
        {
            var organization = await _organizerRepository.GetOwnerOrganization(ownerId)
                ?? throw new ForbiddenException();

            return await _organizerRepository.UpdateProfileImage(organization, image);
        }

        public async Task<IEnumerable<QuizMinimalDto>> GetAvaliableQuizzesForNewHost(int hostId, int organizationId)
        {
            var quizzes = await _organizerRepository.GetAvaliableQuizzesForNewHost(hostId, organizationId);

            return quizzes.Select(x => new QuizMinimalDto(x));
        }

        public async Task<IEnumerable<QuizInvitationDto>> GetOrganizationPendingQuizInvitations(int id)
        {
            var invitations = await _organizerRepository.GetOrganizationPendingQuizInvitations(id);

            return invitations.Select(x => new QuizInvitationDto(x)).ToList();
        }

        public async Task<IEnumerable<HostDto>> GetHostsByQuiz(int quizId)
        {
            return await _organizerRepository.GetHostsByQuiz(quizId);
        }
    }
}
