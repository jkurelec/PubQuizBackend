using System.Collections.Generic;

namespace PubQuizBackend.Util
{
    public static class TeamDeviationProvider
    {
        private static double Q1 { get; set; }
        private static double Q3 { get; set; }
        private static double IQR { get; set; }
        private static double LowerBound { get; set; }
        private static double UpperBound { get; set; }

        public static void SetQuartiles(List<double> list)
        {
            if (list == null || list.Count == 0)
            {
                Q1 = -0.01;
                Q3 = 0.01;
            }
            else
            {
                var sorted = list.OrderBy(x => x).ToList();
                int n = sorted.Count;

                Q1 = GetPercentile(sorted, n, 25);
                Q3 = GetPercentile(sorted, n, 75);
            }

            IQR = Q3 - Q1;
            LowerBound = Q1 - 1.5 * IQR;
            UpperBound = Q3 + 1.5 * IQR;
        }

        private static double GetPercentile(List<double> sortedList, int listCount, double percentile)
        {
            double position = percentile / 100.0 * (listCount - 1);

            int lowerIndex = (int)Math.Floor(position);
            int upperIndex = lowerIndex + 1;
            double fraction = position - lowerIndex;

            return sortedList[lowerIndex] + fraction * (sortedList[upperIndex] - sortedList[lowerIndex]);
        }

        public static bool IsOutlier(double value)
        {
            if (value < LowerBound || value > UpperBound)
                return true;

            return false;
        }
    }
}
