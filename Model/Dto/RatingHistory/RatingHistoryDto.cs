using PubQuizBackend.Util.Interfaces;

namespace PubQuizBackend.Model.Dto.RatingHistory
{
    public class RatingHistoryDto
    {
        public RatingHistoryDto(IRatingHistory ratingHistory)
        {
            Rating = ratingHistory.Rating;
            Date = ratingHistory.Date;
        }

        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}
