using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.RecommendationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IRecommendationService
    {
        Task<EditionFeedbackRequestDto?> GetEditionInfoForFeedback(int userId);
        Task SetUserFeedback(UserFeedbackDto feedback, int userId);
        Task<QuizEditionRecommendationParam> GetEditionRecommendationParams(int editionId);
        Task SetEditionRecommendationParams(QuizEditionRecommendationParam recommendationParam);
        Task<UserRecommendationParam> GetUserRecommendationParams(int userId);
        Task SetUserRecommendationParams(UserRecommendationParam recommendationParam);
    }
}
