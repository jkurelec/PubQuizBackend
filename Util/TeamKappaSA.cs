namespace PubQuizBackend.Util
{
    public class TeamKappaSA
    {
        private static readonly Random rng = new();

        private static float CustomCooling(float tempValue, float acceptanceRateValue)
        {
            float a = 0.8f;
            float b = 1.1f;
            return tempValue * (a + (b - a) * (1 - acceptanceRateValue));
        }

        private static float Sigma(float rAb, float kappa, float scalingFactor)
        {
            float powPos = MathF.Pow(10, rAb / scalingFactor);
            float powNeg = MathF.Pow(10, -rAb / scalingFactor);

            return powPos / (powNeg + kappa + powPos);
        }

        private static float GetWinProbabilityForKappa(int targetRating, int opponentRating, float scalingFactor, float kappa)
        {
            float rAB = targetRating - opponentRating;

            return Sigma(rAB, kappa, scalingFactor);
        }

        private static float CalculatedTeamProbability(float startingProbability, int leaderRating, List<int> teammateRatings, List<float> kappas, int scalingFactor)
        {
            var calculatedProbability = startingProbability;

            for (int i = 0; i < teammateRatings.Count; i++)
            {
                calculatedProbability += GetWinProbabilityForKappa(teammateRatings[i], leaderRating, scalingFactor, kappas[i]);

                if (calculatedProbability < 0.01 || calculatedProbability > 0.99)
                    return Math.Clamp(calculatedProbability, 0.01f, 0.99f);
            }

            return calculatedProbability;
        }

        private static float ObjectiveFunction(float targetProbability, float startingProbability, int leaderRating, List<int> teammateRatings, List<float> kappas, int scalingFactor)
        {
            float calculatedProbability = CalculatedTeamProbability(startingProbability, leaderRating, teammateRatings, kappas, scalingFactor);

            return targetProbability - calculatedProbability;
        }

        public static List<float> CalculateTeamKappas(float targetProbability, float startingProbability, int leaderRating, List<int> teammateRatings, int scalingFactor, float initialTemp, int numIterations, int innerIterations, out float bestFitness)
        {
            var currentSolution = teammateRatings.Select(x => KappaProvider.GetTeamKappa(leaderRating / 50, x / 50)).ToList();
            //Console.WriteLine($"Initial solution: {string.Join(", ", currentSolution)}");
            var bestSolution = currentSolution;
            bestFitness = ObjectiveFunction(targetProbability, startingProbability, leaderRating, teammateRatings, currentSolution, scalingFactor);
            //Console.WriteLine($"Initial fitness: {bestFitness}");

            float temp = initialTemp;
            //Console.WriteLine($"Initial temperature: {temp}\n\n");

            for (int i = 0; i < numIterations; i++)
            {
                var tempValue = temp;
                var acceptedWorse = 0;
                var totalWorse = 0;
                var currentFitness = ObjectiveFunction(targetProbability, startingProbability, leaderRating, teammateRatings, currentSolution, scalingFactor);
                //Console.WriteLine($"\tIteration {i + 1}/{numIterations}, Current fitness: {currentFitness}, Temperature: {temp}\n");

                for (int j = 0; j < innerIterations; j++)
                {
                    var neighborSolution = currentSolution;

                    for (int k = 0; k < neighborSolution.Count; k++)
                    {
                        if (currentFitness > 0)
                            neighborSolution[k] -= rng.NextSingle();
                        else
                            neighborSolution[k] += rng.NextSingle();

                        if (k == 0)
                            neighborSolution[k] = Math.Clamp(neighborSolution[k], 0.01f, float.MaxValue);
                        else
                            neighborSolution[k] = Math.Clamp(neighborSolution[k], 0.01f, neighborSolution[k - 1]);
                    }

                    var neighborFitness = ObjectiveFunction(targetProbability, startingProbability, leaderRating, teammateRatings, neighborSolution, scalingFactor);
                    //Console.WriteLine($"\t\tNeighbor Solution: {string.Join(", ", neighborSolution)}, Fitness: {neighborFitness}");

                    if (neighborFitness == currentFitness)
                        currentSolution = neighborSolution;
                    else
                    {
                        var prob = MathF.Abs(neighborFitness) < MathF.Abs(currentFitness)
                            ? 1
                            : Math.Exp((MathF.Abs(currentFitness) - MathF.Abs(neighborFitness)) / (temp + 1e-9));

                        if (rng.NextDouble() < prob)
                        {
                            //Console.WriteLine($"\t\tAccepting neighbor solution with probability {prob}");
                            if (prob != 1)
                                acceptedWorse++;

                            currentSolution = neighborSolution;
                            currentFitness = neighborFitness;

                            if (MathF.Abs(neighborFitness) < MathF.Abs(bestFitness))
                            {
                                bestSolution = neighborSolution;
                                bestFitness = neighborFitness;

                                //Console.WriteLine($"\t\tNew best solution found: {string.Join(", ", bestSolution)}, Fitness: {bestFitness}");

                                if (MathF.Abs(bestFitness) < 0.01)
                                    return bestSolution;
                            }
                        }
                    }

                }

                var acceptanceRate = totalWorse > 0
                    ? (float)acceptedWorse / totalWorse
                    : 0;

                temp = CustomCooling(tempValue, acceptanceRate);

                //Console.WriteLine("\n\n");

            }

            return bestSolution;
        }
    }
}
