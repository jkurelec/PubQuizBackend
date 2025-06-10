using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using System.Threading.Tasks;

namespace PubQuizBackend.Service.Implementation
{
    public class EloCalculatorService : IEloCalculatorService
    {
        private readonly IEloCalculatorRepository _repository;
        private readonly float TeamKappa = 5f;

        public EloCalculatorService(IEloCalculatorRepository repository)
        {
            _repository = repository;
        }

        public async Task CalculateEditionElo(int editionId, int hostId)
        {
            await _repository.AuthorizeHostByEditionId(editionId, hostId);

            var edition = await _repository.GetEdition(editionId);

            var ratingAction = GetRatingAction(edition);

            if (ratingAction == RatingAction.ABORT)
                throw new BadRequestException("Not all round result are entered!");
            else if (ratingAction == RatingAction.BRIEF)
                await BriefEloCalculation(edition, ratingAction);
            else
                DetailedEloCalculation(edition);

            await _repository.SaveChanges();
        }

        public async Task<int> GetTeamInitialRating(int applicationId)
        {
            await _repository.GetRatingsFromApplication(applicationId);

            //highest rating + elo gained by beating weaker

            return 0;
        }

        private static RatingAction GetRatingAction(QuizEdition edition)
        {
            var detailed = true;
            var allRoundIds = edition.QuizRounds.Select(r => r.Id).ToHashSet();

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var roundResultIds = editionResult.QuizRoundResults.Select(r => r.RoundId).ToHashSet();

                if (!allRoundIds.IsSubsetOf(roundResultIds))
                    return RatingAction.ABORT;

                if (detailed)
                    foreach (var roundResult in editionResult.QuizRoundResults)
                    {
                        if (roundResult.QuizSegmentResults == null || roundResult.QuizSegmentResults.Count == 0)
                        {
                            detailed = false;
                            break;
                        }   
                    }
            }

            return detailed
                ? RatingAction.DETAILED
                : RatingAction.BRIEF;
        }

        private async Task BriefEloCalculation(QuizEdition edition, RatingAction ratingAction)
        {
            Dictionary<int, TeamRatingCalculationParams> teamRatingCalculationParams = new ();
            var editionKFactor = await GetKFactor(edition.Rating, edition.Id, false);
            var scalingFactor = 400.0f;
            var editionRatingUpdate = 0;

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Team.UserTeams.Select(t => t.User).ToList();
                var playerRatings = teamPlayers.Select(p => p.Rating).OrderDescending().ToList();
                var teamRating = playerRatings.Max();


                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, true);

                for (int i = 1; i < playerRatings.Count; i++)
                    teamRating += GetRatingUpdate(teamRating, playerRatings[i], teamKFactor, scalingFactor, Outcome.WIN);

                var wins = (int)editionResult.TotalPoints;
                var losses = (int)edition.TotalPoints - wins;
                var draw = (edition.TotalPoints - editionResult.TotalPoints) % 1 != 0 ? 1 : 0;

                teamRatingCalculationParams.Add(editionResult.Team.Id, new TeamRatingCalculationParams(teamRating, teamKFactor, wins, losses, draw));

                editionRatingUpdate += (int)Math.Round(
                    (
                        losses * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.WIN) +
                        wins * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.LOSS) +
                        draw * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.DRAW)
                    ) / (double)edition.TotalPoints
                );
            }

            edition.Rating += editionRatingUpdate;

            foreach (var teamRating in teamRatingCalculationParams)
            {
                var team = edition.QuizEditionResults.FirstOrDefault(x => x.Id == teamRating.Key)!;

                

                team.Rating = teamRating.Value.TeamRating;
            }
        }

        private static void DetailedEloCalculation(QuizEdition edition)
        {
            // Implement brief elo calculation logic here
        }

        private async Task<int> GetKFactor(int rating, int id, bool team = true)
        {
            var participationsCount = await _repository.GetNumberOfParticipations(id, team);

            int k = (int)Math.Round(800.0 / participationsCount);

            if (k < 10)
                k = 10;
            else if (k > 32)
                k = 32;

            if (rating > 2300)
                k = Math.Min(k, 12);
            else if (rating > 2000)
                k = Math.Min(k, 16);
            else if (rating > 1500)
                k = Math.Min(k, 24);

            return k;
        }

        //private static Task

        public async Task CalculateKappa(CancellationToken cancellationToken = default)
        {
            var kappaData = await _repository.GetAnswersForKappa();
            var kappas = new Dictionary<int, Dictionary<int, float>>();

            foreach (var questionRating in kappaData)
            {
                kappas.Add(questionRating.Key, new Dictionary<int, float>());

                foreach (var answerRating in questionRating.Value)
                {
                    var draws = answerRating.Value.Count(x => x == QuestionResult.Parital);
                    var total = answerRating.Value.Count;

                    kappas[questionRating.Key].Add(
                        answerRating.Key,
                        total > 100
                            ? (float)draws / total
                            : 0.05f
                    );
                }
            }

            KappaProvider.SetKappas(kappas);
        }

        private static float Sigma(float rAb, float kappa, float scalingFactor)
        {
            float powPos = MathF.Pow(10, rAb / scalingFactor);
            float powNeg = MathF.Pow(10, -rAb / scalingFactor);

            return powPos / (powNeg + kappa + powPos);
        }

        private static float GetProbability(int targetRating, int opponentRating, float scalingFactor, int win = 1)
        {
            var kappa = KappaProvider.GetKappa(targetRating / 50, opponentRating / 50);
            float rAB = targetRating - opponentRating;

            return Sigma(win == 1 ? rAB : -rAB, kappa, scalingFactor);
        }

        private static float GetDrawProbability(int targetRating, int opponentRating, float scalingFactor)
        {
            var kappa = KappaProvider.GetKappa(targetRating / 50, opponentRating / 50);
            float rAB = targetRating - opponentRating;

            float sigmaPos = Sigma(rAB, kappa, scalingFactor);
            float sigmaNeg = Sigma(-rAB, kappa, scalingFactor);

            return kappa * MathF.Sqrt(sigmaPos * sigmaNeg);
        }

        private static int GetRatingUpdate(int targetRating, int opponentRating, int kFactor, float scalingFactor, Outcome outcome)
        {
            if (outcome == Outcome.DRAW)
            {
                return (int)(kFactor * (0.5 - GetDrawProbability(targetRating, opponentRating, scalingFactor)));
            }
            else
            {
                var actualScore = outcome == Outcome.WIN ? 1 : 0;

                return (int)(kFactor * (actualScore - GetProbability(targetRating, opponentRating, scalingFactor, actualScore)));
            }
        }

        public static int FindRatingForWinProbability(int editionRating, int initialTeamRating, float targetWinProbability, float scalingFactor,float kappa)
        {
            //var kappa = KappaProvider.GetKappa(editionRating / 50, initialTeamRating / 50);
            var winProbForEqualRatings = 1.0f / (2.0f + kappa);

            if (MathF.Abs(targetWinProbability - winProbForEqualRatings) < float.Epsilon * 100f)
            {
                return 0; // Rating difference is 0 when probabilities match for equal ratings
            }

            var x = (targetWinProbability * kappa + MathF.Sqrt(MathF.Pow(targetWinProbability * kappa, 2) + 4 * targetWinProbability - 4 * MathF.Pow(targetWinProbability, 2)))
                    / (2 - 2 * targetWinProbability);

            var rating = (int)(scalingFactor * MathF.Log10(x));

            return rating;
        }
    }
}
