using PubQuizBackend.Exceptions;
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

        public QuizEditionService(IQuizEditionRepository editionRepository, IOrganizationRepository organizationRepository, IQuizLeagueRepository leagueRepository)
        {
            _editionRepository = editionRepository;
            _organizationRepository = organizationRepository;
            _leagueRepository = leagueRepository;
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

        public async Task<QuizEditionDetailedDto> GetById(int id)
        {
            return new(await _editionRepository.GetByIdDetailed(id));
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id)
        {
            var editions = await _editionRepository.GetByQuizId(id);

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId)
        {
            var host = await _organizationRepository.GetHostByEditionId(userId, editionDto.Id);

            if (!host.EditEdition)
                throw new UnauthorizedException();

            return new(await _editionRepository.Update(editionDto,userId));
        }
    }
}
