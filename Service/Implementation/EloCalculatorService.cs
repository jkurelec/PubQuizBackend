using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PubQuizBackend.Service.Implementation
{
    public class EloCalculatorService : IEloCalculatorService
    {
        private readonly IEloCalculatorRepository _repository;
        private readonly IRatingHistoryService _ratingHistoryService;
        private readonly IQuizLeagueRepository _leagueRepository;

        public EloCalculatorService(IEloCalculatorRepository repository, IRatingHistoryService ratingHistoryService, IQuizLeagueRepository leagueRepository)
        {
            _repository = repository;
            _ratingHistoryService = ratingHistoryService;
            _leagueRepository = leagueRepository;
        }

        public async Task CalculateEditionElo(int editionId, int hostId)
        {
            await _repository.AuthorizeHostByEditionId(editionId, hostId);

            var edition = await _repository.GetEdition(editionId);

            if (edition.Rated)
                throw new BadRequestException("Edition already rated!");

            Console.WriteLine($"Starting Elo Calculation for Edition => Id: {edition.Id}, Name: {edition.Name}");
            var ratingAction = GetRatingAction(edition);
            Console.WriteLine($"Rating Action: {ratingAction}");

            if (ratingAction == RatingAction.ABORT)
                throw new BadRequestException("Not all round result are entered!");
            else if (ratingAction == RatingAction.BRIEF)
                await BriefEloCalculation(edition);
            else
                await DetailedEloCalculation(edition);

            Console.WriteLine($"Elo Calculation completed for Edition => Id: {edition.Id}, Name: {edition.Name}");

            if (edition.LeagueId != null)
            {
                Console.WriteLine($"Adding league round for Edition => Id: {edition.Id}, Name: {edition.Name}");
                var leagueEntries = edition.QuizEditionResults
                    .Select(x => (x.TeamId, x.Rank ?? 0))
                    .ToList();

                await _leagueRepository.AddLeagueRound((int)edition.LeagueId, leagueEntries);
            }

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
            Console.WriteLine("Starting brief Elo calculation...");
            Dictionary<int, TeamRatingCalculationParams> teamRatingCalculationParams = new ();
            var editionKFactor = await GetKFactor(edition.Rating, edition.QuizId, 2);
            Console.WriteLine($"Edition K-Factor: {editionKFactor}");
            var scalingFactor = 400.0f;
            var editionRatingUpdate = 0;

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Users.ToList();
                Console.WriteLine($"Calculating for team: {editionResult.Team.Name} (ID: {editionResult.TeamId})");
                var playerRatings = teamPlayers.Select(p => p.Rating).OrderDescending().ToList();
                Console.WriteLine($"Player Ratings: {string.Join(", ", playerRatings)}");
                var teamRating = playerRatings.Max();

                var kappa = KappaProvider.GetKappa(edition.Rating / 50, teamRating / 50);
                var winProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 1);
                var loseProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 0);
                var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                var totalProbability = winProbability + loseProbability + drawProbability;

                winProbability /= totalProbability;
                drawProbability /= totalProbability;

                //dizati win prob i smanjivati lose prob, draw  sa 0.5 factorom
                //samo digni pa normaliziraj

                var initailPropability = winProbability + (0.5f * drawProbability);
                Console.WriteLine($"Initial Team Rating: {teamRating}, Team Probability: {initailPropability}");

                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);
                var probabilityIncrease = 0f;

                for (int i = 1; i < playerRatings.Count; i++)
                {
                    probabilityIncrease += GetProbabilityForTeam(playerRatings[i], teamRating, scalingFactor);
                    Console.WriteLine($"Probability Increase after Player {i}: {probabilityIncrease}");

                    //teamRatingIncrease += GetTeammateRatingUpdate(teamRating, playerRatings[i], teamKFactor, scalingFactor);
                    //Console.WriteLine($"Team Rating Increase after Player {i}: {teamRatingIncrease}");
                }

                totalProbability += probabilityIncrease;
                winProbability *= (1 + probabilityIncrease);
                drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                winProbability /= totalProbability;
                drawProbability /= totalProbability;

                var newEstimatedProbability = winProbability + (0.5f * drawProbability);

                teamRating = CalculateTargetRating(newEstimatedProbability, edition.Rating, kappa, scalingFactor);
                Console.WriteLine($"Final Team Rating: {teamRating}");

                var wins = (int)editionResult.TotalPoints;
                var losses = (int)edition.TotalPoints - wins;
                var draw = (edition.TotalPoints - editionResult.TotalPoints) % 1 != 0 ? 1 : 0;
                Console.WriteLine($"Wins: {wins}, Losses: {losses}, Draws: {draw}");

                teamRatingCalculationParams.Add(editionResult.Team.Id, new TeamRatingCalculationParams(teamRating, teamKFactor, wins, losses, draw, editionResult.Id, initailPropability));

                editionRatingUpdate += (int)Math.Round(
                    (
                        losses * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.WIN) +
                        wins * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.LOSS) +
                        draw * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.DRAW)
                    ) / (double)edition.TotalPoints
                );
                Console.WriteLine($"Edition Rating Update: {editionRatingUpdate}");
            }

            edition.Rating += editionRatingUpdate;
            Console.WriteLine($"Edition Rating: {edition.Rating}");
            editionRatingUpdate = 0;

            foreach (var teamParams in teamRatingCalculationParams)
            {
                var teamPlayers = edition.QuizEditionResults.FirstOrDefault(x => x.TeamId == teamParams.Key)!.Users;
                var teamRating = teamParams.Value.TeamRating;
                var teamRatingUpdate = 0;
                var teamKFactor = await GetKFactor(teamRating, teamParams.Key, 1);
                Console.WriteLine($"Processing Team: {teamParams.Key}, Team Rating: {teamRating}, Team K-Factor: {teamKFactor}");

                editionRatingUpdate +=
                        teamParams.Value.Losses * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.WIN) +
                        teamParams.Value.Wins * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.LOSS) +
                        teamParams.Value.Draws * GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, Outcome.DRAW);

                teamRatingUpdate += (int)Math.Round(
                    (
                        teamParams.Value.Wins * GetRatingUpdate(teamRating, edition.Rating, teamKFactor, scalingFactor, Outcome.WIN, false) +
                        teamParams.Value.Losses * GetRatingUpdate(teamRating, edition.Rating, teamKFactor, scalingFactor, Outcome.LOSS, false) +
                        teamParams.Value.Draws * GetRatingUpdate(teamRating, edition.Rating, teamKFactor, scalingFactor, Outcome.DRAW, false)
                    ) / (double)edition.TotalPoints
                );
                Console.WriteLine($"Team Rating Update: {teamRatingUpdate}, Edition Rating Update: {editionRatingUpdate}");

                teamRating += teamRatingUpdate;
                edition.QuizEditionResults.FirstOrDefault(x => x.TeamId == teamParams.Key)!.Rating = teamRating;

#region Calculate Team Kappa
                //current team rating probability
                var kappa = KappaProvider.GetKappa(edition.Rating / 50, teamRating / 50);
                var winProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 1);
                var loseProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 0);
                var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                var totalProbability = winProbability + loseProbability + drawProbability;
                winProbability /= totalProbability;
                drawProbability /= totalProbability;
                var estimatedWinPropability = winProbability + (0.5f * drawProbability);

                //actual team result
                var actualScore = (teamParams.Value.Wins + 0.5f * teamParams.Value.Draws) / (float)edition.TotalPoints;
                var deviation = actualScore - estimatedWinPropability;

                await _repository.AddTeamDeviation(
                        new()
                        {
                            Deviation = deviation,
                            EditionResultId = teamParams.Value.EditionResultId,
                        }
                    );

                //ako deviation veci od 0 i outlier onda je vjerojatno preveliki kappa, ako manji onda je vjerojatno premali kappa
                if (TeamDeviationProvider.IsOutlier(deviation) || true)
                {
                    var captainDeviation = actualScore - teamParams.Value.InitialProbability;

                    // ako manji od 0 znaci da je kapetan gori od edicije i sami user ima krivi rating
                    if (captainDeviation > 0)
                    {
                        teamPlayers = teamPlayers.OrderByDescending(x => x.Rating).ToList();
                        var captain = teamPlayers.First();

                        //dodajem devijaciju samo pod premisom da je rating tocan
                        //await _repository.AddTeamDeviation(
                        //    new()
                        //    {
                        //        Deviation = deviation,
                        //        EditionResultId = teamParams.Value.EditionResultId,
                        //    }
                        //);

                        var teamKappas = TeamKappaSA.CalculateTeamKappas(
                            actualScore,
                            teamParams.Value.InitialProbability,
                            captain.Rating,
                            teamPlayers.Select(x => x.Rating).ToList(),
                            (int)scalingFactor,
                            1000,
                            100,
                            10,
                            out var bestFitness
                        );

                        Console.WriteLine($"Kappas: {string.Join(", ", teamKappas)}");
                        Console.WriteLine($"Best fitness: {bestFitness}");

                        if (!TeamDeviationProvider.IsOutlier(bestFitness))
                        {
                            for (int i = 1; i < teamPlayers.Count; i++)
                            {
                                var teammate = teamPlayers.ElementAt(i);
                                await _repository.AddTeamKappa(
                                    new TeamKappa()
                                    {
                                        LeaderElo = captain.Rating / 50,
                                        TeammateElo = teammate.Rating / 50,
                                        CreatedAt = DateTime.UtcNow,
                                        Kappa = teamKappas[i - 1]
                                    }
                                );
                            }
                        }
                    }
                }
#endregion

                //AKO NAJJACI DOBIVA I GUBI VISENAJVJEROJATNIJE GA SVI OVI DRUGI NECE DOSTICI PA JE BOLJE DA SLABIJI SLINGSHOTAJU DA NAJJACEG!K VEC POMAZE U SLINGSHOTANJU ALI MOZDA PROMJENITI KASNIJE!!!
                foreach (var user in teamPlayers)
                {
                    float teamUpdate = teamRatingUpdate;
                    float kFactor = await GetKFactor(user.Rating, user.Id, 0);
                    float ratio = kFactor / teamKFactor;

                    var oldRating = user.Rating;
                    var ratingUpdate = teamUpdate * ratio;
                    Console.WriteLine($"User {user.Username} (ID: {user.Id}) - Initial Rating: {user.Rating}, K-Factor: {kFactor}, Ratio: {ratio}, Rating Update: {ratingUpdate}");

                    user.Rating += (int)Math.Round(ratingUpdate);
                    Console.WriteLine($"User {user.Username} (ID: {user.Id}) - New Rating: {user.Rating}");

                    if (user.Rating < 100)
                        user.Rating = 100;

                    await _ratingHistoryService.AddUserRatingHistories(
                        new()
                        {
                            UserId = user.Id,
                            Date = DateTime.UtcNow,
                            EditionId = edition.Id,
                            RatingChange = (int)Math.Round(ratingUpdate),
                            OldRating = oldRating,
                            NewRating = user.Rating
                        }
                    );
                }
            }

            var oldQuizRating = edition.Quiz.Rating;
            var finalEditionRatingUpdate = (int)Math.Round(editionRatingUpdate / ((double)edition.TotalPoints * edition.QuizEditionResults.Count));
            edition.Rating += finalEditionRatingUpdate;
            Console.WriteLine($"Final Edition Rating Update: {finalEditionRatingUpdate}");

            edition.Quiz.Rating += finalEditionRatingUpdate;
            Console.WriteLine($"Final Quiz Rating Update: {finalEditionRatingUpdate}");

            await _ratingHistoryService.AddQuizRatingHistories(
                new()
                {
                    QuizId = edition.QuizId,
                    Date = DateTime.UtcNow,
                    EditionId = edition.Id,
                    RatingChange = finalEditionRatingUpdate,
                    OldRating = oldQuizRating,
                    NewRating = edition.Quiz.Rating
                }
            );

            edition.Rated = true;

            await _repository.SaveChanges();
        }

        private async Task DetailedEloCalculation(QuizEdition edition)
        {
            var editionKFactor = await GetKFactor(edition.Rating, edition.Id, 2);
            Console.WriteLine($"Edition K-Factor: {editionKFactor}");
            var scalingFactor = 400.0f;
            var editionRatingUpdate = 0;
            var initialProbabilities = new Dictionary<int, float>();

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Users.ToList();
                Console.WriteLine($"Calculating for team: {editionResult.Team.Name} (ID: {editionResult.TeamId})");
                var playerRatings = teamPlayers.Select(p => p.Rating).OrderDescending().ToList();
                Console.WriteLine($"Player Ratings: {string.Join(", ", playerRatings)}");
                var teamRating = playerRatings.Max();

                var kappa = KappaProvider.GetKappa(edition.Rating / 50, teamRating / 50);
                var winProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 1);
                var loseProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 0);
                var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                var totalProbability = winProbability + loseProbability + drawProbability;

                winProbability /= totalProbability;
                drawProbability /= totalProbability;

                //dizati win prob i smanjivati lose prob, draw  sa 0.5 factorom
                //samo digni pa normaliziraj

                var initailPropability = winProbability + (0.5f * drawProbability);
                initialProbabilities.Add(editionResult.TeamId, initailPropability);
                Console.WriteLine($"Initial Team Rating: {teamRating}, Team Probability: {initailPropability}");

                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);
                var probabilityIncrease = 0f;

                for (int i = 1; i < playerRatings.Count; i++)
                {
                    probabilityIncrease += GetProbabilityForTeam(playerRatings[i], teamRating, scalingFactor);
                    Console.WriteLine($"Probability Increase after Player {i}: {probabilityIncrease}");

                    //teamRatingIncrease += GetTeammateRatingUpdate(teamRating, playerRatings[i], teamKFactor, scalingFactor);
                    //Console.WriteLine($"Team Rating Increase after Player {i}: {teamRatingIncrease}");
                }

                totalProbability += probabilityIncrease;
                winProbability *= (1 + probabilityIncrease);
                drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                winProbability /= totalProbability;
                drawProbability /= totalProbability;

                var newEstimatedProbability = winProbability + (0.5f * drawProbability);

                teamRating = CalculateTargetRating(newEstimatedProbability, edition.Rating, kappa, scalingFactor); ;
                Console.WriteLine($"Final Team Rating: {teamRating}");
                Console.WriteLine();

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
                            Console.WriteLine($"Answer Id: {answer.Id}, Outcome: {outcome}");

                            var ratingUpdate = GetRatingUpdate(edition.Rating, teamRating, editionKFactor, scalingFactor, outcome);
                            Console.WriteLine($"Rating Update for Question {answer.Question.Id}: {ratingUpdate}");
                            answer.Question.Rating += ratingUpdate;
                            Console.WriteLine($"New Question Rating {answer.Question.Id}: {answer.Question.Rating}");
                            editionRatingUpdate += ratingUpdate;
                            Console.WriteLine($"Edition Rating Update: {editionRatingUpdate}");
                            Console.WriteLine();
                        }
                    }
                }
                Console.WriteLine();
            }

            var totalQuestions = 0;

            foreach (var round in edition.QuizRounds)
            {
                foreach (var segment in round.QuizSegments)
                {
                    totalQuestions += segment.QuizQuestions.Count;
                }
            }
            Console.WriteLine($"Total Questions: {totalQuestions}");

            var totalTeams = edition.QuizEditionResults.Count;
            Console.WriteLine($"Total Teams: {totalTeams}");
            var totalEntries = totalTeams * totalQuestions;
            Console.WriteLine($"Total Entries: {totalEntries}");
            Console.WriteLine();

            var questionRatingUpdates = new Dictionary<int, int>();

            foreach (var editionResult in edition.QuizEditionResults)
            {
                Console.WriteLine($"Processing Edition Result for Team: {editionResult.Team.Name} (ID: {editionResult.TeamId})");
                var teamPlayers = editionResult.Users.ToList();
                var teamRating = editionResult.Rating;
                Console.WriteLine($"Team Rating: {teamRating}");
                var teamRatingUpdate = 0;
                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);
                Console.WriteLine($"Team K-Factor: {teamKFactor}");
                Console.WriteLine();

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
                                teamRatingUpdate += GetRatingUpdate(teamRating, answer.Question.Rating, teamKFactor, scalingFactor, Outcome.LOSS, false);
                            }
                            else if (answer.Result == (int)QuestionResult.Correct)
                            {
                                questionRatingUpdates[questionId] += GetRatingUpdate(answer.Question.Rating, teamRating, editionKFactor, scalingFactor, Outcome.LOSS);
                                teamRatingUpdate += GetRatingUpdate(teamRating, answer.Question.Rating, teamKFactor, scalingFactor, Outcome.WIN, false);
                            }
                            else
                            {
                                questionRatingUpdates[questionId] += GetRatingUpdate(answer.Question.Rating, teamRating, editionKFactor, scalingFactor, Outcome.DRAW);
                                teamRatingUpdate += GetRatingUpdate(teamRating, answer.Question.Rating, teamKFactor, scalingFactor, Outcome.DRAW, false);
                            }
                            Console.WriteLine($"Question Id: {questionId}, QuestionResult: {(QuestionResult)answer.Result} Team Rating Update: {teamRatingUpdate}, Rating Update: {questionRatingUpdates[questionId]}");
                        }
                    }
                }

                Console.WriteLine();
                var finalTeamRatingUpdate = (int)Math.Round(teamRatingUpdate / (double)totalQuestions);
                Console.WriteLine($"Final Team Rating Update: {finalTeamRatingUpdate}");

                editionResult.Rating += finalTeamRatingUpdate;
                Console.WriteLine($"Final Edition Result Rating for Team {editionResult.Team.Name} (ID: {editionResult.TeamId}): {editionResult.Rating}");
                Console.WriteLine();

                if (editionResult.Rating < 100)
                    editionResult.Rating = 100;

#region Calculate Team Kappa Detailed

                //current team rating probability
                var kappa = KappaProvider.GetKappa(edition.Rating / 50, finalTeamRatingUpdate / 50);
                var winProbability = GetProbability(finalTeamRatingUpdate, edition.Rating, scalingFactor, kappa, 1);
                var loseProbability = GetProbability(finalTeamRatingUpdate, edition.Rating, scalingFactor, kappa, 0);
                var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                var totalProbability = winProbability + loseProbability + drawProbability;
                winProbability /= totalProbability;
                drawProbability /= totalProbability;
                var estimatedWinPropability = winProbability + (0.5f * drawProbability);

                //actual team result
                var actualScore = (float)
                    (editionResult.QuizRoundResults
                        .SelectMany(x => x.QuizSegmentResults)
                        .SelectMany(s => s.QuizAnswers)
                        .Where(a => a.Result == (int)QuestionResult.Correct || a.Result == (int)QuestionResult.Parital)
                        .Sum(a => a.Points) /
                    edition.TotalPoints);

                var deviation = actualScore - estimatedWinPropability;

                await _repository.AddTeamDeviation(
                    new()
                    {
                        Deviation = deviation,
                        EditionResultId = editionResult.Id,
                    }
                );

                //ako deviation veci od 0 i outlier onda je vjerojatno preveliki kappa, ako manji onda je vjerojatno premali kappa
                if (TeamDeviationProvider.IsOutlier(deviation) || true)
                {
                    var captainDeviation = actualScore - initialProbabilities[editionResult.TeamId];

                    // ako manji od 0 znaci da je kapetan gori od edicije i sami user ima krivi rating
                    if (captainDeviation > 0)
                    {
                        teamPlayers = teamPlayers.OrderByDescending(x => x.Rating).ToList();
                        var captain = teamPlayers.First();

                        //dodajem devijaciju samo pod premisom da je rating tocan
                        //await _repository.AddTeamDeviation(
                        //    new()
                        //    {
                        //        Deviation = deviation,
                        //        EditionResultId = editionResult.Id,
                        //    }
                        //);

                        var teamKappas = TeamKappaSA.CalculateTeamKappas(
                            actualScore,
                            initialProbabilities[editionResult.TeamId],
                            captain.Rating,
                            teamPlayers.Select(x => x.Rating).ToList(),
                            (int)scalingFactor,
                            1000,
                            100,
                            10,
                            out var bestFitness
                        );

                        Console.WriteLine($"Kappas: {string.Join(", ", teamKappas)}");
                        Console.WriteLine($"Best fitness: {bestFitness}");

                        if (!TeamDeviationProvider.IsOutlier(bestFitness))
                        {
                            for (int i = 1; i < teamPlayers.Count; i++)
                            {
                                var teammate = teamPlayers.ElementAt(i);
                                await _repository.AddTeamKappa(
                                    new TeamKappa()
                                    {
                                        LeaderElo = captain.Rating / 50,
                                        TeammateElo = teammate.Rating / 50,
                                        CreatedAt = DateTime.UtcNow,
                                        Kappa = teamKappas[i - 1]
                                    }
                                );
                            }
                        }
                    }
                }
#endregion
                foreach (var user in teamPlayers)
                {
                    float teamUpdate = finalTeamRatingUpdate;
                    float kFactor = await GetKFactor(user.Rating, user.Id, 0);
                    float ratio = kFactor / teamKFactor;
                    Console.WriteLine($"User {user.Username} (ID: {user.Id}) - Initial Rating: {user.Rating}, K-Factor: {kFactor}, Ratio: {ratio}");
                    var oldRating = user.Rating;
                    var ratingUpdate = teamUpdate * ratio;

                    user.Rating += (int)Math.Round(ratingUpdate);
                    Console.WriteLine($"User {user.Username} (ID: {user.Id}) - New Rating: {user.Rating}");
                    Console.WriteLine();

                    if (user.Rating < 100)
                        user.Rating = 100;

                    await _ratingHistoryService.AddUserRatingHistories(
                        new()
                        {
                            UserId = user.Id,
                            Date = DateTime.UtcNow,
                            EditionId = edition.Id,
                            RatingChange = (int)Math.Round(ratingUpdate),
                            OldRating = oldRating,
                            NewRating = user.Rating
                        }
                    );
                }
            }

            var editionRating = 0;
            var quizRatingUpdate = 0;

            foreach (var round in edition.QuizRounds)
            {
                foreach (var segment in round.QuizSegments)
                {
                    foreach (var question in segment.QuizQuestions)
                    {
                        questionRatingUpdates.TryGetValue(question.Id, out var questionRatingUpdateTotal);
                        var questionRatingUpdate = questionRatingUpdateTotal / totalTeams;
                        question.Rating += questionRatingUpdate;
                        quizRatingUpdate += questionRatingUpdateTotal;

                        if (question.Rating < 100)
                            question.Rating = 100;

                        editionRating += question.Rating;
                    }
                }
            }
            Console.WriteLine($"Edition Rating Update: {(int)Math.Round(editionRating / (double)totalQuestions) - edition.Rating}");
            var oldQuizRating = edition.Quiz.Rating;
            var finalQuizRatingUpdate = (int)Math.Round(editionRating / (double)totalQuestions) - edition.Rating;/* (int)Math.Round(quizRatingUpdate / (double)totalEntries);*/
            edition.Quiz.Rating += finalQuizRatingUpdate;
            edition.Rating = (int)Math.Round(editionRating / (double)totalQuestions);

            await _ratingHistoryService.AddQuizRatingHistories(
                new()
                {
                    QuizId = edition.QuizId,
                    Date = DateTime.UtcNow,
                    EditionId = edition.Id,
                    RatingChange = finalQuizRatingUpdate,
                    OldRating = oldQuizRating,
                    NewRating = edition.Quiz.Rating
                }
            );

            edition.Rated = true;

            await _repository.SaveChanges();
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
                    var wins = answerRating.Value.Count(x => x == QuestionResult.Correct);
                    var draws = answerRating.Value.Count(x => x == QuestionResult.Parital);
                    var losses = answerRating.Value.Count(x => x == QuestionResult.Incorrect);
                    var total = wins + draws + losses;

                    int divisor = 1;
                    while (draws / (divisor * 10) >= 10)
                    {
                        divisor *= 10;
                    }

                    wins /= divisor;
                    draws /= divisor;
                    losses /= divisor;

                    var sqrt = MathF.Sqrt(wins * losses);
                    kappas[questionRating.Key].Add(
                        answerRating.Key,
                        total > 10000
                            ? draws / (sqrt == 0 ? 1 : sqrt)
                            : 0.05f
                    );
                }
            }

            KappaProvider.SetKappas(kappas);
        }

        public async Task GetTeamKappas(CancellationToken cancellationToken = default)
        {
            KappaProvider.SetTeamKappas(await _repository.GetTeamKappas());
        }

        public async Task GetDeviations(CancellationToken cancellationToken = default)
        {
            TeamDeviationProvider.SetQuartiles(await _repository.GetDeviations());
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

        private static float GetProbability(int targetRating, int opponentRating, float scalingFactor, float kappa, int win = 1)
        {
            float rAB = targetRating - opponentRating;

            return Sigma(win == 1 ? rAB : -rAB, kappa, scalingFactor);
        }

        private static float GetDrawProbability(int targetRating, int opponentRating, float scalingFactor, float kappa)
        {
            float rAB = targetRating - opponentRating;

            float sigmaPos = Sigma(rAB, kappa, scalingFactor);
            float sigmaNeg = Sigma(-rAB, kappa, scalingFactor);

            return kappa * MathF.Sqrt(sigmaPos * sigmaNeg);
        }

        private static float GetDrawProbability(float sigmaPos, float sigmaNeg, float kappa)
        {
            return kappa * MathF.Sqrt(sigmaPos * sigmaNeg);
        }

        private static int GetRatingUpdate(int targetRating, int opponentRating, int kFactor, float scalingFactor, Outcome outcome, bool edition = true)
        {
            //if (outcome == Outcome.DRAW)
            //{
            //    return (int)(kFactor * (0.5 - GetDrawProbability(targetRating, opponentRating, scalingFactor, edition)));
            //}
            //else
            //{
            //    var actualScore = outcome == Outcome.WIN ? 1 : 0;

            //    return (int)(kFactor * (actualScore - GetProbability(targetRating, opponentRating, scalingFactor, actualScore, edition)));
            //}

            var actualScore = outcome == Outcome.WIN ? 1 : outcome == Outcome.DRAW ? 0.5 : 0;
            var kappa = edition ? KappaProvider.GetKappa(targetRating / 50, opponentRating / 50) : KappaProvider.GetKappa(opponentRating / 50, targetRating / 50);

            var winProbability = GetProbability(targetRating, opponentRating, scalingFactor, kappa, 1);
            var loseProbability = GetProbability(targetRating, opponentRating, scalingFactor, kappa, 0);
            var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);

            var totalProbability = winProbability + loseProbability + drawProbability;

            winProbability /= totalProbability;
            drawProbability /= totalProbability;

            //return (int)(kFactor * (actualScore - (GetProbability(targetRating, opponentRating, scalingFactor, kappa, 1) + (0.5 * GetDrawProbability(targetRating, opponentRating, scalingFactor, kappa)))));
            return (int)(kFactor * (actualScore - (winProbability + (0.5 * drawProbability))));
        }

        // kad se s ovim racuna team elo prejako skoci u granicnim slucajevima 0.98 => 0.99 elo skoci za stotine il tisuce
        //koristiti nakon sto ima neki smisleni kappa
        public static int FindRatingChangeForProbability(float targetWinProbability, float scalingFactor, float kappa)
        {
            var winProbForEqualRatings = 1.0f / (2.0f + kappa);

            if (MathF.Abs(targetWinProbability - winProbForEqualRatings) < float.Epsilon * 100f)
            {
                return 0;
            }

            targetWinProbability = Math.Min(targetWinProbability, 0.999f);

            var x = (-targetWinProbability * kappa + MathF.Sqrt(MathF.Pow(targetWinProbability * kappa, 2) + 4 * targetWinProbability - 4 * MathF.Pow(targetWinProbability, 2)))
                    / (2 - 2 * targetWinProbability);

            var rating = (int)(scalingFactor * MathF.Log10(x));

            if (rating < 0)
            {
                Console.WriteLine($"X: {x}\nTarget Win Probability: {targetWinProbability}\nKappa: {kappa}\n-targetWinProbability * kappa: {-targetWinProbability * kappa}\nMathF.Pow(targetWinProbability * kappa, 2): {MathF.Pow(targetWinProbability * kappa, 2)}\n4 * targetWinProbability: {4 * targetWinProbability}\n- 4 * MathF.Pow(targetWinProbability, 2): {-4 * MathF.Pow(targetWinProbability, 2)}\n(2 - 2 * targetWinProbability): {(2 - 2 * targetWinProbability)}");
                Console.WriteLine($"-targetWinProbability * kappa: {-targetWinProbability * kappa}\nSqrt: {MathF.Sqrt(MathF.Pow(targetWinProbability * kappa, 2) + 4 * targetWinProbability - 4 * MathF.Pow(targetWinProbability, 2))}");
                Console.WriteLine($"X: {x}, Log10X: {MathF.Log10(x)}, Rating: {rating}");
                return 0;
            }
                

            return rating;
        }

        //zakljuco al mozda ce poslje trebati
        public static int CalculateTargetRating(float probability, int opponentRating, float kappa, float scalingFactor)
        {
            float numerator = -kappa * (probability - 0.5f) +
                              MathF.Sqrt(MathF.Pow(kappa * (probability - 0.5f), 2) - 4 * (probability - 1) * probability);
            float denominator = 2 - 2 * probability;
            float a = numerator / denominator;

            float targetRating = opponentRating + scalingFactor * MathF.Log10(a);

            return (int)MathF.Floor(targetRating);
        }

        private static float GetProbabilityForTeam(int targetRating, int opponentRating, float scalingFactor)
        {
            var kappa = KappaProvider.GetTeamKappa(opponentRating / 50, targetRating / 50);
            float rAB = targetRating - opponentRating;

            return Sigma(rAB, kappa, scalingFactor);
        }

        private static float GetWinProbabilityForKappa(int targetRating, int opponentRating, float scalingFactor, float kappa)
        {
            float rAB = targetRating - opponentRating;

            return Sigma(rAB, kappa, scalingFactor);
        }

        private static float GetTeammateProbability(int targetRating, int opponentRating, float scalingFactor, float kappa)
        {
            float rAB = targetRating - opponentRating;

            return Sigma(rAB, kappa, scalingFactor);
        }

        private static int GetTeammateRatingUpdate(int targetRating, int opponentRating, int kFactor, float scalingFactor)
        {
            var kappa = KappaProvider.GetTeamKappa(targetRating / 50, opponentRating / 50);

            var leaderWinRatingUpdate = GetTeammateProbability(targetRating, opponentRating, scalingFactor, kappa);
            var teammateWinRatingUpdate = GetTeammateProbability(opponentRating, targetRating, scalingFactor, kappa);
            // nemoguce da leaderWinRatingUpdate bude 0
            return (int)(kFactor * (teammateWinRatingUpdate / leaderWinRatingUpdate));
        }

        private static float CalculateTeamKappa(float targetProbability, int targetRating, int opponentRating, float scalingFactor, int maxIters = 100)
        {
            float probability(float kappa)
            {
                var winProbability = GetProbability(targetRating, opponentRating, scalingFactor, kappa, 1);
                var loseProbability = GetProbability(targetRating, opponentRating, scalingFactor, kappa, 0);
                var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);

                return winProbability + (0.5f * drawProbability);
            }

            var lowerBound = 0.01f;
            var upperBound = KappaProvider.GetHighestTeamKappa();
            
            while (probability(upperBound) > targetProbability)
            {
                lowerBound = upperBound;
                upperBound *= 2f;
            }

            for (int i = 0; i < maxIters; i++)
            {
                var targetKappa = (lowerBound + upperBound) / 2f;
                var calculatedProbability = probability(targetKappa);
                var probabilityDifference = targetProbability - calculatedProbability;

                if (Math.Abs(probabilityDifference) < 0.01f)
                {
                    Console.WriteLine($"Found target Kappa: {targetKappa} for Target Probability: {targetProbability} after {i + 1} iterations for Team Rating: {targetRating} and Edition Rating: {opponentRating}");
                    return MathF.Round(targetKappa, 2);
                }
                else if (probabilityDifference > 0)
                {
                    upperBound = targetKappa;
                }
                else
                {
                    lowerBound = targetKappa;
                }
            }

            return 0f;
        }

        private static float GetTeamProbability(int targetRating, int opponentRating, float scalingFactor)
        {
            var kappa = KappaProvider.GetKappa(opponentRating / 50, targetRating / 50);

            var winProbability = GetProbability(targetRating, opponentRating, scalingFactor, kappa, 1);
            var loseProbability = GetProbability(targetRating, opponentRating, scalingFactor, kappa, 0);
            var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);

            return winProbability + (0.5f * drawProbability);
        }

        public async Task<Dictionary<string, float>> GetProbability(int editionId)
        {
            var probabilities = new Dictionary<string, float>();
            var edition = await _repository.GetEdition(editionId);
            var editionKFactor = await GetKFactor(edition.Rating, edition.QuizId, 2);
            var scalingFactor = 400.0f;

            foreach (var editionResult in edition.QuizEditionResults)
            {
                var teamPlayers = editionResult.Users.ToList();
                var playerRatings = teamPlayers.Select(p => p.Rating).OrderDescending().ToList();
                var teamRating = playerRatings.Max();

                var kappa = KappaProvider.GetKappa(edition.Rating / 50, teamRating / 50);
                var winProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 1);
                var loseProbability = GetProbability(teamRating, edition.Rating, scalingFactor, kappa, 0);
                var drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                var totalProbability = winProbability + loseProbability + drawProbability;

                winProbability /= totalProbability;
                drawProbability /= totalProbability;

                var initailPropability = winProbability + (0.5f * drawProbability);
                //Console.WriteLine($"Kappa :{kappa}");
                //Console.WriteLine($"team {editionResult.Team.Name} initial probability {initailPropability}");
                //Console.WriteLine($"Win probability: {winProbability}, Draw probability: {drawProbability}, Lose probability: {loseProbability}, Sum probability: {totalProbability}");
                var teamKFactor = await GetKFactor(teamRating, editionResult.Team.Id, 1);
                var probabilityIncrease = 0f;

                for (int i = 1; i < playerRatings.Count; i++)
                {
                    probabilityIncrease += GetProbabilityForTeam(playerRatings[i], teamRating, scalingFactor);
                    //Console.WriteLine($"Probability Increase: {probabilityIncrease} after player with rating: {playerRatings[i]}");
                }

                totalProbability += probabilityIncrease;
                winProbability *= (1 + probabilityIncrease);
                drawProbability = 0.5f * GetDrawProbability(winProbability, loseProbability, kappa);
                winProbability /= totalProbability;
                drawProbability /= totalProbability;

                var newEstimatedProbability = winProbability + (0.5f * drawProbability);
                //Console.WriteLine($"team {editionResult.Team.Name} new estimated probability {newEstimatedProbability}");
                //Console.WriteLine($"Win probability: {winProbability}, Draw probability: {drawProbability}, Lose probability: {loseProbability}, Sum probability: {totalProbability}");
                probabilities.Add(editionResult.Team.Name, newEstimatedProbability);
            }

            return probabilities;
        }
    }
}
