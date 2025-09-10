namespace PubQuizBackend.Util
{
    public static class AverageDeltaCalculator
    {
        public static float GetDelta(float currentAverage, double newValue, int totalCount)
        {
            
            return (float)(currentAverage + (newValue - currentAverage) / totalCount);
        }
    }
}
