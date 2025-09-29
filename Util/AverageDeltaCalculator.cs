namespace PubQuizBackend.Util
{
    public static class AverageDeltaCalculator
    {
        public static float GetDelta(float currentAverage, double newValue, int totalCount)
        {
            if (currentAverage == 0 && totalCount == 1)
                return (float)newValue;

            return (float)(currentAverage + (newValue - currentAverage) / totalCount);
        }
    }
}
