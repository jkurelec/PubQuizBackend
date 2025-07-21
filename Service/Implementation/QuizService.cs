using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Service.Implementation
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _repository;
        private readonly MediaServerClient _mediaServerClient;

        public QuizService(IQuizRepository repository, MediaServerClient mediaServerClient)
        {
            _repository = repository;
            _mediaServerClient = mediaServerClient;
        }

        public async Task<QuizDetailedDto> Add(NewQuizDto quizDto, int hostId)
        {
            return new(await _repository.Add(quizDto, hostId));
        }

        public async Task<bool> Delete(int id, int hostId)
        {
            return await _repository.Delete(id, hostId);
        }

        public async Task<IEnumerable<object>> GetAll(bool detailed = false)
        {
            return detailed
                ? await _repository.GetAllDetailed().ContinueWith(x => x.Result.Select(x => new QuizDetailedDto(x)))
                : await _repository.GetAll().ContinueWith(x => x.Result.Select(x => new QuizBriefDto(x)));
        }

        public async Task<object> GetById(int id, bool detailed = false)
        {
            return detailed
                ? await _repository.GetByIdDetailed(id).ContinueWith(x => new QuizDetailedDto(x.Result))
                : await _repository.GetById(id).ContinueWith(x => new QuizBriefDto(x.Result));
        }

        public async Task<IEnumerable<QuizMinimalDto>> GetByHostAndOrganization(int hostId, int organizationId)
        {
            var quizzes = await _repository.GetByHostAndOrganization(hostId, organizationId);

            return quizzes.Select(x => new QuizMinimalDto(x)).ToList();
        }

        public async Task<QuizDetailedDto> Update(NewQuizDto quizDto, int hostId)
        {
            return new(await _repository.Update(quizDto, hostId));
        }

        public async Task<string> UpdateProfileImage(IFormFile image, int quizId, int hostId)
        {

            var quiz = await _repository.GetOwnerQuizByIds(hostId, quizId);

            var fileName = await _mediaServerClient.PostImage($"/private/update/quiz", image, $"{quiz.Id}{Path.GetExtension(image.FileName)}");

            if (quiz.ProfileImage != fileName)
            {
                quiz.ProfileImage = fileName;

                await _repository.Save();
            }

            return fileName;
        }
    }
}
