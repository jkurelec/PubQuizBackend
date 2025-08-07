using NuGet.Protocol.Core.Types;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Other;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizEditionService : IQuizEditionService
    {
        private readonly IQuizEditionRepository _editionRepository;
        private readonly IOrganizationRepository _organizationRepository; 
        private readonly IQuizLeagueRepository _leagueRepository;
        private readonly MediaServerClient _mediaServerClient;

        public QuizEditionService(IQuizEditionRepository editionRepository, IOrganizationRepository organizationRepository, IQuizLeagueRepository leagueRepository, MediaServerClient mediaServerClient)
        {
            _editionRepository = editionRepository;
            _organizationRepository = organizationRepository;
            _leagueRepository = leagueRepository;
            _mediaServerClient = mediaServerClient;
        }

        public async Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId)
        {
            var organizer = await _organizationRepository.GetHost(userId, editionDto.QuizId);

            if (!organizer.CreateEdition)
                throw new ForbiddenException();

            if (editionDto.LeagueId != null)
            {
                var league = await _leagueRepository.GetBriefById(editionDto.LeagueId.Value);

                if (league.QuizId != editionDto.QuizId)
                    throw new ForbiddenException();
            }

            if (userId != editionDto.HostId)
            {
                var hostExists = await _organizationRepository.DoesHostExist(editionDto.HostId, editionDto.QuizId);

                if (!hostExists)
                    throw new BadRequestException("Who is the host?");
            }

            var hosts = await _organizationRepository.GetHostsByQuiz(editionDto.QuizId);

            await _mediaServerClient.AddEditionPermissionAsync(
                new EditionPermissionDto
                {
                    EditionId = editionDto.Id,
                    UserIds = hosts.Select(x => x.UserBrief.Id).ToList(),
                });

            return new (await _editionRepository.Add(editionDto, userId));
        }

        public async Task Delete(int editionId, int userId)
        {
            var edition = await _editionRepository.GetById(editionId);
            var host = await _organizationRepository.GetHost(userId, edition.QuizId);

            if (!host.DeleteEdition)
                throw new ForbiddenException();

            if (edition.Time < DateTime.UtcNow)
                throw new BadRequestException("You cannot delete past editions!");

            await _editionRepository.Delete(edition);
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetAll()
        {
            var editions = await _editionRepository.GetAll();

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> GetById(int id, int? userId = null)
        {
            var edition = await _editionRepository.GetByIdDetailed(id, userId);

            if (edition.Time < DateTime.UtcNow)
                return new PastQuizEditionDetailedDto(edition);

            return new(edition);
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id)
        {
            var editions = await _editionRepository.GetByQuizId(id);

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<int> GetTotalCount(EditionTimeFilter filter)
        {
            return await _editionRepository.GetTotalCount(filter);
        }

        public async Task<IEnumerable<QuizEditionMinimalDto>> GetPage(int page, int pageSize, EditionFilter editionFilter)
        {
            var editions = await _editionRepository.GetPage(page, pageSize, editionFilter);

            return editions.Select(x => new QuizEditionMinimalDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionMinimalDto>> GetUpcomingCompletedPage(int page, int pageSize, EditionFilter editionFilter, bool upcoming = true)
        {
            var editions = await _editionRepository.GetUpcomingCompletedPage(page, pageSize, editionFilter, upcoming);

            return editions.Select(x => new QuizEditionMinimalDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId)
        {
            var host = await _organizationRepository.GetHostByEditionId(userId, editionDto.Id);

            if (!host.EditEdition)
                throw new ForbiddenException();

            return new(await _editionRepository.Update(editionDto,userId));
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetByLocationId(int locationId)
        {
            var editions = await _editionRepository.GetByLocationId(locationId);

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<string> UpdateProfileImage(IFormFile image, int editionId, int hostId)
        {

            var edition = await _editionRepository.GetById(editionId);
            var host = await _organizationRepository.GetHostDto(hostId, edition.QuizId);

            if (!host.HostPermissions.EditEdition || !host.HostPermissions.CreateEdition)
                throw new ForbiddenException();

            var fileName = await _mediaServerClient.PostImage($"/private/update/edition", image, $"{edition.Id}{Path.GetExtension(image.FileName)}");

            if (edition.ProfileImage != fileName)
            {
                edition.ProfileImage = fileName;
                await _editionRepository.Save();
            }

            return fileName;
        }

        public async Task<bool?> HasDetailedQuestions(int editionId)
        {
            return await _editionRepository.HasDetailedQuestions(editionId);
        }

        public async Task SetDetailedQuestions(int editionId, int userId, bool detailed)
        {
            var host = await _organizationRepository.GetHostByEditionId(userId, editionId);

            if (!host.CrudQuestion)
                throw new ForbiddenException();

            await _editionRepository.SetDetailedQuestions(editionId, userId, detailed);
        }
    }
}
