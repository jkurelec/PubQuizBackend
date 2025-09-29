using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.RecommendationDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IRecommendationRepository: IBaseRepository
    {
        Task<QuizEditionRecommendationParam> GetEditionRecommendationParams(int editionId);
        Task SetEditionRecommendationParams(QuizEditionRecommendationParam recommendationParam);
        Task<UserRecommendationParam> GetUserRecommendationParams(int userId);
        Task SetUserRecommendationParams(UserRecommendationParam recommendationParam);
        Task<IEnumerable<UserTopRecommendation>> GetRecommendations(int userId);
    }
}
