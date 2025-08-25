namespace PubQuizBackend.Util
{
    public static class KappaProvider
    {
        private static Dictionary<int, Dictionary<int, float>> Kappas = new();
        private static Dictionary<int, Dictionary<int, float>> TeamKappas = new();

        public static void SetKappas(Dictionary<int, Dictionary<int, float>> kappas)
        {
            Kappas = kappas;
        }

        public static float GetKappa(int questionRatingBucket, int answerRatingBucket)
        {
            if (!Kappas.TryGetValue(questionRatingBucket, out var answerRatings))
                return 0.05f;

            if (!answerRatings.TryGetValue(answerRatingBucket, out var kappa))
                return 0.05f;

            return kappa;
        }

        public static float GetHighestKappa()
        {
            if (Kappas.Count == 0)
                return 0.05f;

            return Kappas.Values
                .Where(x => x.Count > 0)
                .SelectMany(x => x.Values)
                .DefaultIfEmpty(0.05f)
                .Max();
        }

        public static void SetTeamKappas(Dictionary<int, Dictionary<int, float>> teamKappas)
        {
            TeamKappas = teamKappas;
        }

        public static float GetTeamKappa(int teamRatingBucket, int playerRatingBucket)
        {
            if (!TeamKappas.TryGetValue(teamRatingBucket, out var playerRatings))
                return 0.5f;

            if (!playerRatings.TryGetValue(playerRatingBucket, out var kappa))
                return 0.5f;

            return kappa;
        }

        public static float GetHighestTeamKappa()
        {
            if (TeamKappas.Count == 0)
                return 0.5f;

            return TeamKappas.Values
                .Where(x => x.Count > 0)
                .SelectMany(x => x.Values)
                .DefaultIfEmpty(0.5f)
                .Max();
        }
    }
}
