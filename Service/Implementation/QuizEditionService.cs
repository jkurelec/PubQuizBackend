using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizEditionService : IQuizEditionService
    {
        private readonly IQuizEditionRepository _editionRepository;
        private readonly IOrganizationRepository _organizationRepository; 
        private readonly IQuizLeagueRepository _leagueRepository;
        private readonly ITeamRepository _teamRepository;

        public QuizEditionService(IQuizEditionRepository editionRepository, IOrganizationRepository organizationRepository, IQuizLeagueRepository leagueRepository, ITeamRepository teamRepository)
        {
            _editionRepository = editionRepository;
            _organizationRepository = organizationRepository;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
        }

        public async Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId)
        {
            var organizer = await _organizationRepository.GetHost(userId, editionDto.QuizId);

            if (!organizer.CreateEdition)
                throw new UnauthorizedException();

            if (editionDto.LeagueId != null)
            {
                var league = await _leagueRepository.GetById(editionDto.LeagueId.Value);

                if (league.QuizId != editionDto.QuizId)
                    throw new UnauthorizedException();
            }

            if (userId != editionDto.HostId)
            {
                var hostExists = await _organizationRepository.DoesHostExist(editionDto.HostId, editionDto.QuizId);

                if (!hostExists)
                    throw new BadRequestException("Who is the host?");
            }


            return new (await _editionRepository.Add(editionDto, userId));
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

        public async Task Delete(int editionId, int userId)
        {
            var edition = await _editionRepository.GetById(editionId);
            var host = await _organizationRepository.GetHost(userId, edition.QuizId);

            if (!host.DeleteEdition)
                throw new UnauthorizedException();

            await _editionRepository.Delete(edition);
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetAll()
        {
            var editions = await _editionRepository.GetAll();

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionApplicationDto>> GetApplications(int editionId, int hostId, bool unanswered)
        {
            var edition = await _editionRepository.GetById(editionId);
            var host = await _organizationRepository.GetHost(hostId, edition.QuizId);

            var applications = await _editionRepository.GetApplications(editionId, hostId, unanswered);

            return applications.Select(x => new QuizEditionApplicationDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> GetById(int id)
        {
            return new(await _editionRepository.GetByIdDetailed(id));
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id)
        {
            var editions = await _editionRepository.GetByQuizId(id);

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
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

        public async Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId)
        {
            var edition = await _editionRepository.GetById(editionDto.Id);
            var host = await _organizationRepository.GetHost(userId, edition.QuizId);

            if (!host.EditEdition)
                throw new UnauthorizedException();

            return new(await _editionRepository.Update(editionDto,userId));
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
