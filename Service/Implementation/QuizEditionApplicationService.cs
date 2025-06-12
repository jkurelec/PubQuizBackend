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

        public QuizEditionApplicationService(IQuizEditionRepository editionRepository, IOrganizationRepository organizationRepository, ITeamRepository teamRepository)
        {
            _editionRepository = editionRepository;
            _organizationRepository = organizationRepository;
            _teamRepository = teamRepository;
        }

        public async Task ApplyTeam(QuizEditionApplicationRequestDto application, int registrantId)
        {
            if (!application.UserIds.Any())
                throw new BadRequestException("No members in team!");

            var registrant = await _teamRepository.GetMemeberById(registrantId, application.TeamId);

            if (registrant.RegisterTeam == false)
                throw new UnauthorizedException();

            var usersInTeam = await _teamRepository.FilterMemberIdsInTeam(application.UserIds, application.TeamId);

            var usersNotInTeam = application.UserIds.Except(usersInTeam).ToList();

            if (usersNotInTeam.Count > 0)
                throw new BadRequestException("User/s not in team!");


            await _editionRepository.ApplyTeam(application.EditionId, application.TeamId, application.UserIds, registrantId);
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
                throw new UnauthorizedException();

            await _editionRepository.RemoveTeamFromEdition(editionId, teamId);
        }

        public async Task RespondToApplication(QuizEditionApplicationResponseDto application, int hostId)
        {
            await _editionRepository.RespondToApplication(application.ApplicationId, application.Response, hostId);
        }

        public async Task WithdrawFromEdition(int editionId, int teamId, int userId)
        {
            var teamMember = await _teamRepository.GetMemeberById(userId, teamId);

            if (!teamMember.RegisterTeam)
                throw new UnauthorizedException();

            await _editionRepository.WithdrawFromEdition(editionId, teamId);
        }
    }
}
