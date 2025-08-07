using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Service.Implementation
{
    public class EloCalculatorService : IEloCalculatorService
    {
        private readonly IEloCalculatorRepository _repository;
        private readonly IRatingHistoryService _ratingHistoryService;

        public EloCalculatorService(IEloCalculatorRepository repository, IRatingHistoryService ratingHistoryService)
        {
            _repository = repository;
            _ratingHistoryService = ratingHistoryService;
        }

        public async Task CalculateEditionElo(int editionId, int hostId)
        {
            await _repository.AuthorizeHostByEditionId(editionId, hostId);

            var edition = await _repository.GetEdition(editionId);

            if (edition.Rated)
                throw new BadRequestException("Edition already rated!");

            var ratingAction = GetRatingAction(edition);

            if (ratingAction == RatingAction.ABORT)
                throw new BadRequestException("Not all round result are entered!");
            else if (ratingAction == RatingAction.BRIEF)
                await BriefEloCalculation(edition);
            else
                await DetailedEloCalculation(edition);

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

        private async Task BriefEloCalculation(QuizEdition edition)
        {
            Dictionary<int, TeamRatingCalculationParams> teamRatingCalculationParams = new ();
            var editionKFactor = await GetKFactor(edition.Rating, edition.QuizId, 2);
            var scalingFactor = 400.0f;
            var editionRatingUpdate = 0;

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Users.ToList();
                var playerRatings = teamPlayers.Select(p => p.Rating).OrderDescending().ToList();
                var teamRating = playerRatings.Max();
                var teamRatingIncrease = 0;
                var initailWinPropability = GetProbability(edition.Rating, teamRating, scalingFactor, 1, false);

                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);

                for (int i = 1; i < playerRatings.Count; i++)
                {
                    var probabilityIncrease = GetProbabilityForTeam(teamRating, playerRatings[i], scalingFactor);
                    
                    teamRatingIncrease += FindRatingChangeForWinProbability(edition.Rating, teamRating, initailWinPropability * (1 + probabilityIncrease), scalingFactor);
                }

                teamRating += teamRatingIncrease;

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
            editionRatingUpdate = 0;

            foreach (var teamParams in teamRatingCalculationParams)
            {
                var teamPlayers = edition.QuizEditionResults.FirstOrDefault(x => x.TeamId == teamParams.Key)!.Users;
                var teamRating = teamParams.Value.TeamRating;
                var teamRatingUpdate = 0;
                var teamKFactor = await GetKFactor(teamRating, teamParams.Key, 1);

                editionRatingUpdate +=
                        teamParams.Value.Losses * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.WIN) +
                        teamParams.Value.Wins * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.LOSS) +
                        teamParams.Value.Draws * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.DRAW);

                teamRatingUpdate += (int)Math.Round(
                    (
                        teamParams.Value.Wins * GetRatingUpdate(edition.Rating, teamRating, teamKFactor, scalingFactor, Outcome.WIN, false) +
                        teamParams.Value.Losses * GetRatingUpdate(edition.Rating, teamRating, teamKFactor, scalingFactor, Outcome.LOSS, false) +
                        teamParams.Value.Draws * GetRatingUpdate(edition.Rating, teamRating, teamKFactor, scalingFactor, Outcome.DRAW, false)
                    ) / (double)edition.TotalPoints
                );
                
                edition.QuizEditionResults.FirstOrDefault(x => x.TeamId == teamParams.Key)!.Rating = teamRating + teamRatingUpdate;

                //AKO NAJJACI DOBIVA I GUBI VISENAJVJEROJATNIJE GA SVI OVI DRUGI NECE DOSTICI PA JE BOLJE DA SLABIJI SLINGSHOTAJU DA NAJJACEG! K VEC POMAZE U SLINGSHOTANJU ALI MOZDA PROMJENITI KASNIJE!!!

                foreach (var user in teamPlayers)
                {
                    float teamUpdate = teamRatingUpdate;
                    float kFactor = await GetKFactor(user.Rating, user.Id, 0);
                    float ratio = kFactor / teamKFactor;

                    var ratingUpdate = teamUpdate * ratio;

                    user.Rating += (int)Math.Round(ratingUpdate);

                    if (user.Rating < 100)
                        user.Rating = 100;

                    await _ratingHistoryService.AddUserRatingHistories(
                        new ()
                        {
                            UserId = user.Id,
                            Rating = user.Rating,
                            Date = DateTime.UtcNow
                        }
                    );
                }
            }

            var finalEditionRatingUpdate = (int)Math.Round(editionRatingUpdate / ((double)edition.TotalPoints * edition.QuizEditionResults.Count));
            edition.Rating += finalEditionRatingUpdate;

            edition.Quiz.Rating += finalEditionRatingUpdate;

            await _ratingHistoryService.AddQuizRatingHistories(
                new()
                {
                    QuizId = edition.QuizId,
                    Rating = edition.Quiz.Rating,
                    Date = DateTime.UtcNow
                }
            );

            edition.Rated = true;

            await _repository.SaveChanges();
        }

        private async Task DetailedEloCalculation(QuizEdition edition)
        {
            var editionKFactor = await GetKFactor(edition.Rating, edition.Id, 2);
            var scalingFactor = 400.0f;
            var editionRatingUpdate = 0;

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Users.ToList();
                var playerRatings = teamPlayers.Select(p => p.Rating).OrderDescending().ToList();
                var teamRating = playerRatings.Max();
                var teamRatingUpdate = 0;
                var initailWinPropability = GetProbability(edition.Rating, teamRating, scalingFactor, 1, false);

                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);

                for (int i = 1; i < playerRatings.Count; i++)
                {
                    var probabilityIncrease = GetProbabilityForTeam(teamRating, playerRatings[i], scalingFactor);

                    teamRatingUpdate += FindRatingChangeForWinProbability(edition.Rating, teamRating, initailWinPropability * (1 + probabilityIncrease), scalingFactor);
                }

                teamRating += teamRatingUpdate;

                editionResult.Rating = teamRating;

                foreach (var round in editionResult.QuizRoundResults)
                {
                    foreach (var segment in round.QuizSegmentResults)
                    {
                        foreach (var answer in segment.QuizAnswers)
                        {
                            var outcome = Outcome.DRAW;

                            if (answer.Result == (int)QuestionResult.Incorrect)
                                outcome = Outcome.WIN;
                            else if (answer.Result == (int)QuestionResult.Correct)
                                outcome = Outcome.LOSS;
                            else
                                outcome = Outcome.DRAW;

                            var ratingUpdate = GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, outcome);
                            answer.Question.Rating = edition.Rating + ratingUpdate;
                            editionRatingUpdate += ratingUpdate;
                        }
                    }
                }
            }

            var totalQuestions = 0;

            foreach (var round in edition.QuizRounds)
            {
                foreach (var segment in round.QuizSegments)
                {
                    totalQuestions += segment.QuizQuestions.Count;
                }
            }

            var totalTeams = edition.QuizEditionResults.Count;
            var totalEntries = totalTeams * totalQuestions;

            var questionRatingUpdates = new Dictionary<int, int>();

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Users.ToList();
                var teamRating = editionResult.Rating;
                var teamRatingUpdate = 0;
                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);

                foreach (var round in editionResult.QuizRoundResults)
                {
                    foreach (var segment in round.QuizSegmentResults)
                    {
                        foreach (var answer in segment.QuizAnswers)
                        {
                            var questionId = answer.QuestionId;

                            if (!questionRatingUpdates.ContainsKey(questionId))
                                questionRatingUpdates[questionId] = 0;

                            if (answer.Result == (int)QuestionResult.Incorrect)
                            {
                                questionRatingUpdates[questionId] += GetRatingUpdate(answer.Question.Rating, teamRating, editionKFactor, scalingFactor, Outcome.WIN);
                                teamRatingUpdate += GetRatingUpdate(edition.Rating, teamRating, teamKFactor, scalingFactor, Outcome.LOSS, false);
                            }
                            else if (answer.Result == (int)QuestionResult.Correct)
                            {
                                questionRatingUpdates[questionId] += GetRatingUpdate(answer.Question.Rating, teamRating, editionKFactor, scalingFactor, Outcome.LOSS);
                                teamRatingUpdate += GetRatingUpdate(edition.Rating, teamRating, teamKFactor, scalingFactor, Outcome.WIN, false);
                            }
                            else
                            {
                                questionRatingUpdates[questionId] += GetRatingUpdate(answer.Question.Rating, teamRating, editionKFactor, scalingFactor, Outcome.DRAW); ;
                                teamRatingUpdate += GetRatingUpdate(edition.Rating, teamRating, teamKFactor, scalingFactor, Outcome.DRAW, false);
                            }
                        }
                    }
                }

                var finalTeamRatingUpdate = (int)Math.Round(teamRatingUpdate / (double)totalQuestions);

                editionResult.Rating += finalTeamRatingUpdate;

                if (editionResult.Rating < 100)
                    editionResult.Rating = 100;

                foreach (var user in teamPlayers)
                {
                    float teamUpdate = finalTeamRatingUpdate;
                    float kFactor = await GetKFactor(user.Rating, user.Id, 0);
                    float ratio = kFactor / teamKFactor;

                    var ratingUpdate = teamUpdate * ratio;

                    user.Rating += (int)Math.Round(ratingUpdate);

                    if (user.Rating < 100)
                        user.Rating = 100;

                    await _ratingHistoryService.AddUserRatingHistories(
                        new()
                        {
                            UserId = user.Id,
                            Rating = user.Rating,
                            Date = DateTime.UtcNow
                        }
                    );
                }

                var editionRating = 0;
                var quizRatingUpdate = 0;

                foreach (var round in edition.QuizRounds)
                {
                    foreach (var segment in round.QuizSegments)
                    {
                        foreach (var question in segment.QuizQuestions)
                        {
                            questionRatingUpdates.TryGetValue(question.Id, out var questionRatingUpdate);

                            question.Rating += questionRatingUpdate;
                            quizRatingUpdate += questionRatingUpdate;

                            if (question.Rating < 100)
                                question.Rating = 100;

                            editionRating += question.Rating;
                        }
                    }
                }

                edition.Rating = (int)Math.Round(editionRating / (double)totalQuestions);

                var finalQuiznRatingUpdate = (int)Math.Round(quizRatingUpdate / (double)totalQuestions);
                edition.Quiz.Rating += finalQuiznRatingUpdate;

                await _ratingHistoryService.AddQuizRatingHistories(
                    new()
                    {
                        QuizId = edition.QuizId,
                        Rating = edition.Quiz.Rating,
                        Date = DateTime.UtcNow
                    }
                );

                edition.Rated = true;

                await _repository.SaveChanges();
            }
        }

        private async Task<int> GetKFactor(int rating, int id, int entity)
        {
            var participationsCount = await _repository.GetNumberOfParticipations(id, entity);

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
                    var total = answerRating.Value.Count;
                    var draws = answerRating.Value.Count(x => x == QuestionResult.Parital);
                    var winsAndLosses = total - draws;

                    kappas[questionRating.Key].Add(
                        answerRating.Key,
                        total > 100
                            ? (float)draws / winsAndLosses
                            : 0.05f
                    );
                }
            }

            KappaProvider.SetKappas(kappas);
        }

        public async Task<bool> IsEditionRated(int editionId)
        {
            return await _repository.IsEditionRated(editionId);
        }

        private static float Sigma(float rAb, float kappa, float scalingFactor)
        {
            float powPos = MathF.Pow(10, rAb / scalingFactor);
            float powNeg = MathF.Pow(10, -rAb / scalingFactor);

            return powPos / (powNeg + kappa + powPos);
        }

        private static float GetProbability(int targetRating, int opponentRating, float scalingFactor, int win = 1, bool edition = true)
        {
            var kappa = edition ? KappaProvider.GetKappa(targetRating / 50, opponentRating / 50) : KappaProvider.GetKappa(opponentRating / 50, targetRating / 50);
            float rAB = targetRating - opponentRating;

            return Sigma(win == 1 ? rAB : -rAB, kappa, scalingFactor);
        }

        private static float GetDrawProbability(int targetRating, int opponentRating, float scalingFactor, bool edition = true)
        {
            var kappa = edition ? KappaProvider.GetKappa(targetRating / 50, opponentRating / 50) : KappaProvider.GetKappa(opponentRating / 50, targetRating / 50);
            float rAB = targetRating - opponentRating;

            float sigmaPos = Sigma(rAB, kappa, scalingFactor);
            float sigmaNeg = Sigma(-rAB, kappa, scalingFactor);

            return kappa * MathF.Sqrt(sigmaPos * sigmaNeg);
        }

        private static int GetRatingUpdate(int targetRating, int opponentRating, int kFactor, float scalingFactor, Outcome outcome, bool edition = true)
        {
            if (outcome == Outcome.DRAW)
            {
                return (int)(kFactor * (0.5 - GetDrawProbability(targetRating, opponentRating, scalingFactor, edition)));
            }
            else
            {
                var actualScore = outcome == Outcome.WIN ? 1 : 0;

                return (int)(kFactor * (actualScore - GetProbability(targetRating, opponentRating, scalingFactor, actualScore, edition)));
            }
        }

        public static int FindRatingChangeForWinProbability(int editionRating, int initialTeamRating, float targetWinProbability, float scalingFactor)
        {
            var kappa = KappaProvider.GetKappa(editionRating / 50, initialTeamRating / 50);
            var winProbForEqualRatings = 1.0f / (2.0f + kappa);

            if (MathF.Abs(targetWinProbability - winProbForEqualRatings) < float.Epsilon * 100f)
            {
                return 0;
            }

            var x = (targetWinProbability * kappa + MathF.Sqrt(MathF.Pow(targetWinProbability * kappa, 2) + 4 * targetWinProbability - 4 * MathF.Pow(targetWinProbability, 2)))
                    / (2 - 2 * targetWinProbability);

            var rating = (int)(scalingFactor * MathF.Log10(x));

            return rating;
        }

        private static float GetProbabilityForTeam(int targetRating, int opponentRating, float scalingFactor)
        {
            float rAB = targetRating - opponentRating;

            return Sigma(-rAB, 5f, scalingFactor);
        }
    }
}
