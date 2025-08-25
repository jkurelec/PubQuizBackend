namespace PubQuizBackend.Model.Dto.TeamDto
{
    public class TeamRatingCalculationParams
    {
        public int TeamRating { get; set; }
        public int TeamKFactor { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int EditionResultId { get; set; }
        public float InitialProbability { get; set; }

        public TeamRatingCalculationParams(int teamRating, int teamKFactor, int wins, int losses, int draws, int editionResultId, float initialProbability)
        {
            TeamRating = teamRating;
            TeamKFactor = teamKFactor;
            Wins = wins;
            Losses = losses;
            Draws = draws;
            EditionResultId = editionResultId;
            InitialProbability = initialProbability;
        }
    }
}
