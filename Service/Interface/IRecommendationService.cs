using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Service.Interface
{
    public interface IRecommendationService
    {
        Task<QuizEditionRecommendationParam> GetEditionRecommendationParams(int editionId);
        Task SetEditionRecommendationParams(QuizEditionRecommendationParam recommendationParam);
        Task<UserRecommendationParam> GetUserRecommendationParams(int userId);
        Task SetUserRecommendationParams(UserRecommendationParam recommendationParam);
    }
}
