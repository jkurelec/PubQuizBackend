using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Model.Dto.RecommendationDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _repository;
        private readonly IQuizEditionRepository _quizEditionRepository;
        private readonly IUserRepository _userRepository;

        public RecommendationService(IRecommendationRepository repository, IQuizEditionRepository quizEditionRepository, IUserRepository userRepository)
        {
            _repository = repository;
            _quizEditionRepository = quizEditionRepository;
            _userRepository = userRepository;
        }

        public Task<QuizEditionRecommendationParamsDto> GetEditionRecommendationParams(int editionId)
        {
            throw new NotImplementedException();
        }

        public async Task SetEditionRecommendationParams(int editionId)
        {
            var editionRecommendationInfo = await _quizEditionRepository.GetRecommendationInfoById(editionId);
            
        }

        public async Task<EditionFeedbackRequestDto?> GetEditionInfoForFeedback(int userId)
        {
            return await _repository.GetEditionInfoForFeedback(userId);
        }

        Task IRecommendationService.SetEditionRecommendationParams(int editionId)
        {
            return SetEditionRecommendationParams(editionId);
        }
    }
}
