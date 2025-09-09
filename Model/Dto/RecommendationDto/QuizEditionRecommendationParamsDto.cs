using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.RecommendationDto
{
    public class QuizEditionRecommendationParamsDto
    {
        public int EditionId { get; set; }
        public int Rating { get; set; }
        public int RatingDifference { get; set; }
        public int Duration { get; set; }
        public int CategoryId { get; set; }
        public int HostId { get; set; }
        public int NumberOfTeams { get; set; }
        public int TeamSize { get; set; }
        public TimeOnly TimeOfEdition { get; set; }
        public DayOfWeek DayOfTheWeek { get; set; }
    }
}
