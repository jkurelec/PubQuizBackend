namespace PubQuizAttendeeFrontend.Models.Dto.RecommendationDto
{
    public class EditionFeedbackRequestDto
    {
        public int EditionId { get; set; }
        public string EditionName { get; set; } = string.Empty;
        public string HostUsername { get; set; } = string.Empty;
    }
}
