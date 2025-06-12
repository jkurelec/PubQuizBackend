namespace PubQuizBackend.Model.Dto.TeamDto
{
    public class TeamRatingCalculationParams
    {
        public int TeamRating { get; set; }
        public int TeamKFactor { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public TeamRatingCalculationParams(int teamRating, int teamKFactor, int wins, int losses, int draws)
        {
            TeamRating = teamRating;
            TeamKFactor = teamKFactor;
            Wins = wins;
            Losses = losses;
            Draws = draws;
        }
    }
}
