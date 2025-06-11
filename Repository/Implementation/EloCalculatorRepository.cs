using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class EloCalculatorRepository : IEloCalculatorRepository
    {
        private readonly PubQuizContext _context;

        public EloCalculatorRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task AuthorizeHostByEditionId(int editionId, int hostId)
        {
            var hostOfEdition = await _context.QuizEditions
                .AnyAsync(x => x.Id == editionId && x.HostId == hostId);

            if (!hostOfEdition)
                throw new UnauthorizedException();
        }

        public async Task<Dictionary<int, Dictionary<int, List<QuestionResult>>>> GetAnswersForKappa()
        {
            var result = await _context.QuizQuestions
                .Where(x => x.QuizAnswers.Any())
                .Select(x => new
                {
                    QuestionRatingBucket = x.Rating / 50,
                    Answers = x.QuizAnswers.Select(a => new
                    {
                        EditionRatingBucket = a.SegmentResult.RoundResult.EditionResult.Rating / 50,
                        a.Result
                    }).ToList()
                })
                .ToListAsync();

            var dictionary = result
                .GroupBy(x => x.QuestionRatingBucket)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(x => x.Answers)
                          .GroupBy(a => a.EditionRatingBucket)
                          .ToDictionary(
                              ag => ag.Key,
                              ag => ag.Select(a => EnumConverter.FromInt<QuestionResult>(a.Result)).ToList()
                          )
                );

            return dictionary;
        }

        public async Task<QuizEdition> GetEdition(int editionId)
        {
            return await _context.QuizEditions
                .Include(x => x.QuizRounds)
                .Include(x => x.QuizEditionResults)
                    .ThenInclude(e => e.Team)
                        .ThenInclude(t => t.UserTeams)
                            .ThenInclude(u => u.User)
                .Include(x => x.QuizEditionResults)
                    .ThenInclude(e => e.QuizRoundResults)
                        .ThenInclude(r => r.QuizSegmentResults)
                            .ThenInclude(s => s.QuizAnswers)
                .FirstOrDefaultAsync(x => x.Id == editionId)
                ?? throw new NotFoundException($"Edition with id => {editionId} not found!");
        }

        public async Task<int> GetNumberOfParticipations(int id, int entity)
        {
            return entity switch
            {
                0 => await _context.QuizEditionResults
                                        .Where(x => x.Edition.Time < DateTime.UtcNow)
                                        .CountAsync(x => x.Team.UserTeams.Any(x => x.UserId == id)),
                1 => await _context.QuizEditionResults
                                        .Where(x => x.Edition.Time < DateTime.UtcNow)
                                        .CountAsync(x => x.TeamId == id),
                2 => await _context.QuizEditions
                                        .Where(x => x.Time < DateTime.UtcNow)
                                        .CountAsync(x => x.QuizId == id),
                _ => throw new BadRequestException("Unknown entity type."),
            };
        }

        public async Task<IEnumerable<int>> GetRatingsFromApplication(int applicationId)
        {
            var ratings = await _context.QuizEditionApplications
                .Where(x => x.Id == applicationId)
                .SelectMany(x => x.Users.Select(u => u.Rating))
                .ToListAsync();

            if (ratings.Count == 0)
                throw new NotFoundException($"No ratings found for application with id => {applicationId}!");

            return ratings;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
