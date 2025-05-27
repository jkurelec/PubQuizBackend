using Microsoft.AspNetCore.Http.HttpResults;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizEditionService : IQuizEditionService
    {
        private readonly IQuizEditionRepository _repository;

        public QuizEditionService(IQuizEditionRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId)
        {
            return new (await _repository.Add(editionDto, userId));
        }

        public async Task ApplyTeam(QuizEditionApplicationRequestDto application, int registrantId)
        {
            await _repository.ApplyTeam(application.EditionId, application.TeamId, application.UserIds, registrantId);
        }

        public async Task Delete(int editionId, int userId)
        {
            await _repository.Delete(editionId, userId);
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetAll()
        {
            var editions = await _repository.GetAll();

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task<IEnumerable<QuizEditionApplicationDto>> GetApplications(int editionId, int hostId, bool unanswered)
        {
            var applications = await _repository.GetApplications(editionId, hostId, unanswered);

            return applications.Select(x => new QuizEditionApplicationDto(x)).ToList();
        }

        public async Task<QuizEditionDetailedDto> GetById(int id)
        {
            return new(await _repository.GetById(id));
        }

        public async Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id)
        {
            var editions = await _repository.GetByQuizId(id);

            return editions.Select(x => new QuizEditionBriefDto(x)).ToList();
        }

        public async Task RespondToApplication(QuizEditionApplicationResponseDto application, int hostId)
        {
            await _repository.RespondToApplication(application.ApplicationId, application.Response, hostId);
        }

        public async Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId)
        {
            return new(await _repository.Update(editionDto,userId));
        }
    }
}
