namespace PubQuizBackend.Model.Dto.RecommendationDto
{
    public class UserFeedbackDto
    {
        public int UserId { get; set; }
        public int EditionId { get; set; }
        public int GeneralRating { get; set; }
        public int HostRating { get; set; }
        public int DifficultyRating { get; set; }
        public int DurationRating { get; set; }
        public int NumberOfPeopleRating { get; set; }
    }
}
