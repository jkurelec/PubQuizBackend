namespace PubQuizBackend.Util
{
    public static class KappaProvider
    {
        private static Dictionary<int, Dictionary<int, float>> Kappas = new();

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
    }
}
