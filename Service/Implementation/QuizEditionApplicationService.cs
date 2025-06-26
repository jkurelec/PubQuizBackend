using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizEditionApplicationService : IQuizEditionApplicationService
    {
        private readonly IQuizEditionRepository _editionRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IQuizEditionApplicationRepository _applicationRepository;

        public QuizEditionApplicationService(IQuizEditionRepository editionRepository, IOrganizationRepository organizationRepository, ITeamRepository teamRepository, IQuizEditionApplicationRepository applicationRepository)
        {
            _editionRepository = editionRepository;
            _organizationRepository = organizationRepository;
            _teamRepository = teamRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task ApplyTeam(QuizEditionApplicationRequestDto application, int registrantId)
        {
            if (!application.UserIds.Any())
                throw new BadRequestException("No members in team!");

            var registrant = await _teamRepository.GetMemeberById(registrantId, application.TeamId);

            if (registrant.RegisterTeam == false)
                throw new ForbiddenException();

            var takenSpots = await _applicationRepository.GetAcceptedCountByEditionId(application.EditionId);
            var totalSpots = await _applicationRepository.GetMaxTeamsByEditionId(application.EditionId);

            if (totalSpots == takenSpots)
                throw new BadRequestException("No more spots available for this edition!");

            var usersInTeam = await _teamRepository.FilterMemberIdsInTeam(application.UserIds, application.TeamId);

            var usersNotInTeam = application.UserIds.Except(usersInTeam).ToList();

            if (usersNotInTeam.Count > 0)
                throw new BadRequestException("User/s not in team!");


            await _editionRepository.ApplyTeam(application.EditionId, application.TeamId, application.UserIds, registrantId);
        }

        public async Task<bool> CheckIfUserApplied(int userId, int editionId)
        {
            return await _applicationRepository.CheckIfUserApplied(userId, editionId);
        }

        public async Task<IEnumerable<AcceptedQuizEditionApplicationDto>> GetAcceptedApplicationsByEdition(int id)
        {
            var applications = await _applicationRepository.GetByEditionId(id, true);

            return applications.Select(x => new AcceptedQuizEditionApplicationDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionApplicationDto>> GetApplications(int editionId, int hostId, bool unanswered)
        {
            var edition = await _editionRepository.GetById(editionId);
            var host = await _organizationRepository.GetHost(hostId, edition.QuizId);

            var applications = await _editionRepository.GetApplications(editionId, hostId, unanswered);

            return applications.Select(x => new QuizEditionApplicationDto(x)).ToList();
        }

        public async Task RemoveTeamFromEdition(int editionId, int teamId, int userId)
        {
            var host = await _organizationRepository.GetHostByEditionId(userId, editionId);

            if (!host.ManageApplication)
                throw new ForbiddenException();

            await _editionRepository.RemoveTeamFromEdition(editionId, teamId);
        }

        public async Task RespondToApplication(QuizEditionApplicationResponseDto applicationDto, int hostId)
        {
            var application = await _applicationRepository.GetApplicationById(applicationDto.ApplicationId);
            var takenSpots = await _applicationRepository.GetAcceptedCountByEditionId(application.EditionId);
            var totalSpots = await _applicationRepository.GetMaxTeamsByEditionId(application.EditionId);

            if (totalSpots == takenSpots)
                throw new BadRequestException("No more spots available for this edition!");

            await _editionRepository.RespondToApplication(applicationDto.ApplicationId, applicationDto.Response, hostId);
        }

        public async Task WithdrawFromEdition(int editionId, int teamId, int userId)
        {
            var teamMember = await _teamRepository.GetMemeberById(userId, teamId);

            if (!teamMember.RegisterTeam)
                throw new ForbiddenException();

            await _editionRepository.WithdrawFromEdition(editionId, teamId);
        }
    }
}
