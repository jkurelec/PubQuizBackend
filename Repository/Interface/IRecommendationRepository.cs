using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Model.Dto.RecommendationDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IRecommendationRepository
    {
        Task<EditionFeedbackRequestDto?> GetEditionInfoForFeedback(int editionId);
        Task<QuizEditionRecommendationParamsDto> GetEditionRecommendationParams(int editionId);
        Task SetEditionRecommendationParams(UserFeedbackDto feedback);
    }
}
