namespace PubQuizBackend.Util.Interfaces
{
    public interface IRatingHistory
    {
        public DateTime Date { get; set; }
        public int RatingChange { get; set; }
        public int? EditionId { get; set; }
        public int? OldRating { get; set; }
        public int? NewRating { get; set; }
    }
}
