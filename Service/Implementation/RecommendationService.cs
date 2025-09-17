using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.RecommendationDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _repository;

        public RecommendationService(IRecommendationRepository repository)
        {
            _repository = repository;
        }

        public async Task<EditionFeedbackRequestDto?> GetEditionInfoForFeedback(int userId)
        {
            return await _repository.GetEditionInfoForFeedback(userId);
        }

        public async Task SetUserFeedback(UserFeedbackDto feedback, int userId)
        {
            if (feedback.UserId != userId)
                throw new ForbiddenException();

            await _repository.SetUserFeedback(feedback);
            await _repository.Save();
        }

        public async Task<QuizEditionRecommendationParam> GetEditionRecommendationParams(int editionId)
        {
            return await _repository.GetEditionRecommendationParams(editionId);
        }

        public async Task SetEditionRecommendationParams(QuizEditionRecommendationParam recommendationParam)
        {
            await _repository.SetEditionRecommendationParams(recommendationParam);
            await _repository.Save();
            var saved = await _repository.GetEditionRecommendationParams(recommendationParam.EditionId);
        }

        public async Task<UserRecommendationParam> GetUserRecommendationParams(int userId)
        {
            return await _repository.GetUserRecommendationParams(userId);
        }

        public async Task SetUserRecommendationParams(UserRecommendationParam recommendationParam)
        {
            await _repository.SetUserRecommendationParams(recommendationParam);
            await _repository.Save();
        }
    }
}
