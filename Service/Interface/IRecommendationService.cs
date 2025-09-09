using PubQuizAttendeeFrontend.Models.Dto.RecommendationDto;
using PubQuizBackend.Model.Dto.RecommendationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IRecommendationService
    {
        Task<EditionFeedbackRequestDto?> GetEditionInfoForFeedback(int userId);
        Task SetEditionRecommendationParams(int editionId);
        Task<QuizEditionRecommendationParamsDto> GetEditionRecommendationParams(int editionId);
    }
}
