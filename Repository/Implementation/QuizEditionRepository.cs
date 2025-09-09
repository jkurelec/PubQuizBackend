using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Dto.RecommendationDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using System.Text;
using System.Text.RegularExpressions;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizEditionRepository : IQuizEditionRepository
    {
        private readonly IMemoryCache _cache;
        private readonly PubQuizContext _context;
        private readonly IPrizeRepository _prizeRepository;

        public QuizEditionRepository(IMemoryCache cache, PubQuizContext context, IPrizeRepository prizeRepository)
        {
            _cache = cache;
            _context = context;
            _prizeRepository = prizeRepository;
        }

        public async Task<QuizEdition> Add(NewQuizEditionDto editionDto, int userId)
        {
            if (await _context.QuizEditions.AnyAsync(x => x.QuizId == editionDto.QuizId && x.Name == editionDto.Name))
            {
                var altName = GetAltName(editionDto);

                throw new ConflictException($"Name already taken! Suggestion: {altName}");
            }

            if (!await _context.QuizCategories.AnyAsync(x => x.Id == editionDto.CategoryId))
                throw new BadRequestException("Category?");

            if (!await _context.Locations.AnyAsync(x => x.Id == editionDto.LocationId))
                throw new BadRequestException("Location?");

            if (editionDto.MaxTeams < 2)
                throw new BadRequestException("Max teams must be at least 2!");

            DateAndFeeCheck(editionDto);

            var quiz = await _context.Quizzes.FindAsync(editionDto.QuizId)
                ?? throw new TotalnoSiToPromislioException();

            editionDto.Rating = quiz.Rating;

            editionDto.Id = 0;

            var prizes = new List<EditionPrize>();

            if (editionDto.Visibility < 0 || editionDto.Visibility > 2)
                throw new BadRequestException("Invalid visibility!");

            var edition = editionDto.ToObject();

            edition.ProfileImage = "default.jpg";

            var entity = await _context.QuizEditions.AddAsync(edition);
            await _context.SaveChangesAsync();

            foreach (var prizeDto in editionDto.Prizes)
                prizes.Add(await _prizeRepository.AddEdition(prizeDto, entity.Entity.Id));

            return await GetByIdDetailed(entity.Entity.Id);
        }

        public async Task Delete(QuizEdition edition)
        {
            _context.QuizEditions.Remove(edition);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizEdition>> GetAll()
        {
            return await _context.QuizEditions
                .Include(x => x.Category)
                .Include(x => x.Quiz)
                .Include(x => x.Location)
                .ThenInclude(l => l.City)
                .ThenInclude(c => c.Country)
                .ToListAsync();
        }

        public async Task<QuizEdition> GetById(int id)
        {
            return await _context.QuizEditions.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ForbiddenException();
        }

        public async Task<QuizEdition> GetByIdDetailed(int id, int? userId = null)
        {
            var edition = await _context.QuizEditions
                .Include(x => x.Host)
                .Include(x => x.Category)
                .Include(x => x.Location).ThenInclude(l => l.City).ThenInclude(c => c.Country)
                .Include(x => x.Quiz)
                .Include(x => x.League)
                .Include(x => x.EditionPrizes)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("Edition not found!");

            if (edition.Time < DateTime.UtcNow)
            {
                var results = await _context.QuizEditionResults
                    .Include(x => x.QuizRoundResults)
                    .Include(x => x.Team)
                    .Where(x => x.EditionId == id)
                    .ToListAsync();

                edition.QuizEditionResults = results;

                if ((Visibility)edition.Visibility == Visibility.ONLY_ATTENDEE && userId != null)
                {
                    var teamResults = await _context.QuizEditionResults
                        .Include(x => x.QuizRoundResults)
                            .ThenInclude(r => r.QuizSegmentResults)
                                .ThenInclude(s => s.QuizAnswers)
                        .Where(x => x.Users.Any(u => u.Id == userId) && x.EditionId == id)
                        .FirstOrDefaultAsync();

                    if (teamResults != null)
                    {
                        if (edition.Time.Date < DateTime.UtcNow.Date)
                            edition.QuizRounds = await GetRoundsWithAnswers(id);
                        //else
                        //    teamResults.QuizRoundResults
                        //        .SelectMany(r => r.QuizSegmentResults)
                        //        .SelectMany(s => s.QuizAnswers)
                        //        .ToList()
                        //        .ForEach(a => a.Answer = "");

                        var existing = edition.QuizEditionResults.FirstOrDefault(r => r.Id == teamResults.Id);

                        if (existing != null)
                            edition.QuizEditionResults.Remove(existing);

                        edition.QuizEditionResults.Add(teamResults);
                    }
                }

                if ((Visibility)edition.Visibility == Visibility.VISIBLE && edition.Time.Date < DateTime.UtcNow.Date)
                    edition.QuizRounds = await GetRoundsWithAnswers(id);
            }

            return edition;
        }

        public async Task<IEnumerable<QuizEdition>> GetByQuizId(int id)
        {
            return await _context.QuizEditions
                .Include(x => x.Category)
                .Include(x => x.Quiz)
                .Include(x => x.Location)
                .ThenInclude(l => l.City)
                .ThenInclude(c => c.Country)
                .Where(x => x.QuizId == id)
                .ToListAsync()
                    ?? throw new NotFoundException("Edition not found!");
        }

        public async Task<IEnumerable<QuizEdition>> GetPage(int page, int pageSize, EditionFilter editionFilter)
        {
            IQueryable<QuizEdition> query = _context.QuizEditions
                .AsNoTracking()
                .Include(x => x.Category);

            query = editionFilter switch
            {
                EditionFilter.NEWEST => query.OrderByDescending(x => x.Time),
                EditionFilter.OLDEST => query.OrderBy(x => x.Time),
                EditionFilter.HIGHEST_RATED => query.OrderByDescending(x => x.Rating),
                EditionFilter.LOWEST_RATED => query.OrderBy(x => x.Rating),
                _ => query.OrderByDescending(x => x.Time)
            };

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<QuizEdition>> GetUpcomingCompletedPage(int page, int pageSize, EditionFilter editionFilter, bool upcoming = true)
        {
            IQueryable<QuizEdition> query = _context.QuizEditions
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => upcoming
                    ? x.Time > DateTime.UtcNow
                    : x.Time < DateTime.UtcNow);

            query = editionFilter switch
            {
                EditionFilter.NEWEST => query.OrderByDescending(x => x.Time),
                EditionFilter.OLDEST => query.OrderBy(x => x.Time),
                EditionFilter.HIGHEST_RATED => query.OrderByDescending(x => x.Rating),
                EditionFilter.LOWEST_RATED => query.OrderBy(x => x.Rating),
                _ => query.OrderByDescending(x => x.Time)
            };

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task<int> GetTotalCount(EditionTimeFilter filter)
        {
            string cacheKey = filter switch
            {
                EditionTimeFilter.UPCOMING => "QuizEditionTotalCount_Upcoming",
                EditionTimeFilter.PAST => "QuizEditionTotalCount_Past",
                _ => "QuizEditionTotalCount_All"
            };

            if (!_cache.TryGetValue(cacheKey, out int count))
            {
                IQueryable<QuizEdition> query = _context.QuizEditions;

                switch (filter)
                {
                    case EditionTimeFilter.UPCOMING:
                        query = query.Where(q => q.Time > DateTime.UtcNow);
                        break;
                    case EditionTimeFilter.PAST:
                        query = query.Where(q => q.Time <= DateTime.UtcNow);
                        break;
                    case EditionTimeFilter.ALL:
                    default:
                        break;
                }

                count = await query.CountAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, count, cacheEntryOptions);
            }

            return count;
        }

        public async Task<bool> IsNameTaken(string name, int quizId)
        {
            return await _context.QuizEditions.AnyAsync(x => x.Name == name && x.QuizId == quizId);
        }

        public async Task<QuizEdition> Update(NewQuizEditionDto editionDto, int userId)
        {
            var edition = await _context.QuizEditions.Include(x => x.EditionPrizes).FirstOrDefaultAsync(x => x.Id == editionDto.Id && x.QuizId == editionDto.QuizId)
                ?? throw new BadRequestException();

            if (edition.Name != editionDto.Name)
            {
                var nameTaken = await IsNameTaken(editionDto.Name, edition.QuizId);

                if (!nameTaken)
                    edition.Name = editionDto.Name;
                else
                    throw new BadRequestException("Name already taken!");
            }

            if (edition.QuizId != editionDto.QuizId)
                throw new BadRequestException();

            if (edition.HostId != editionDto.HostId)
            {
                var validHost = await _context.HostOrganizationQuizzes.AnyAsync(x => x.HostId == editionDto.HostId && x.QuizId == editionDto.QuizId);

                    if (validHost)
                    edition.HostId = editionDto.HostId;
                else
                    throw new BadRequestException("Invalid host!");
            }

            if (edition.CategoryId != editionDto.CategoryId)
            {
                var validCateogry = await _context.QuizCategories.AnyAsync(x => x.Id == editionDto.CategoryId);

                if (validCateogry)
                    edition.CategoryId = editionDto.CategoryId;
                else
                    throw new BadRequestException("Invalid category!");
            }

            if (edition.LocationId != editionDto.LocationId)
            {
                var validLocation = await _context.Locations.AnyAsync(x => x.Id == editionDto.LocationId);

                if (validLocation)
                    edition.LocationId = editionDto.LocationId;
                else
                    throw new BadRequestException("Invalid location!");
            }

            if (edition.LeagueId != editionDto.LeagueId)
            {
                var validLeague = await _context.QuizLeagues.AnyAsync(x => x.Id == editionDto.LeagueId);

                if (validLeague)
                    edition.LeagueId = editionDto.LeagueId;
                else
                    throw new BadRequestException("Invalid league!");
            }

            if (editionDto.Visibility < 0 || editionDto.Visibility > 2)
                throw new BadRequestException("Invalid visibility!");

            if (editionDto.MaxTeams < 2 && editionDto.MaxTeams >= edition.AcceptedTeams)
                throw new BadRequestException("Max teams must be at least 2 and more or equal to accepted teams!");

            DateAndFeeCheck(editionDto, true);

            PropertyUpdater.UpdateEntityFromDtoSpecificFields(
                edition,
                editionDto,
                [
                    "FeeType", "Fee", "Duration", "MaxTeamSize", "Description",
                    "Time", "RegistrationStart", "RegistrationEnd"
                ]
            );

            var prizesChanged = await UpdatePrizes(edition, editionDto);

            if (_context.Entry(edition).State == EntityState.Unchanged && !prizesChanged)
                throw new BadRequestException("Super update!");

            await _context.SaveChangesAsync();

            return await GetByIdDetailed(edition.Id);
        }

        public async Task ApplyTeam(int editionId, int teamId, IEnumerable<int> userIds, int registrantId)
        {
            var teamRegistered = await _context.QuizEditionApplications
                .AnyAsync(x => x.TeamId == teamId && x.EditionId == editionId);

            if (teamRegistered)
                throw new BadRequestException("Team already applied to this edition");

            var userRegistered = await _context.Users
                .FromSqlInterpolated($@"
                    SELECT u.* FROM ""user"" u
                    INNER JOIN quiz_edition_application_user qau ON qau.user_id = u.id
                    INNER JOIN quiz_edition_application qea ON qea.id = qau.application_id
                    WHERE qea.edition_id = {editionId} AND u.id = ANY({userIds.ToArray()})
                    LIMIT 1")
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            if (userRegistered != null)
                throw new BadRequestException($"User {userRegistered} already applied for this edition!");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _context.QuizEditionApplications.AddAsync(new QuizEditionApplication
                {
                    TeamId = teamId,
                    EditionId = editionId,
                    Accepted = null
                });

                await _context.SaveChangesAsync();

                foreach (var userId in userIds)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO quiz_edition_application_user " +
                        "(application_id, user_id) VALUES ({0}, {1})",
                        entity.Entity.Id, userId
                    );
                }

                var edition = await GetById(editionId);

                edition.PendingTeams += 1;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new DivineException();
            }
        }

        public async Task<IEnumerable<QuizEditionApplication>> GetApplications(int editionId, int hostId, bool unanswered)
        {
            var query = _context.QuizEditionApplications.Where(x => x.EditionId == editionId);

            if (unanswered)
            {
                query = query.Where(x => x.Accepted == null);
            }

            var applications = await query
                .Include(x => x.Team)
                    .ThenInclude(t => t.Category)
                .Include(x => x.Team)
                    .ThenInclude(t => t.Quiz)
                .Include(x => x.Users)
                .ToListAsync();

            return applications;
        }

        public async Task RespondToApplication(int applicationId, bool applicationResponse, int hostId)
        {
            var application = await _context.QuizEditionApplications.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == applicationId)
                ?? throw new ForbiddenException();

            if (application.Accepted != null)
                throw new ForbiddenException();

            var edition = await _context.QuizEditions.FirstOrDefaultAsync(x => x.Id == application.EditionId)
                ?? throw new ForbiddenException();

            //var host = await _context.HostOrganizationQuizzes.Where(x => x.HostId == hostId && x.QuizId == edition.QuizId).FirstOrDefaultAsync()
            //    ?? throw new ForbiddenException();

            //if (!host.ManageApplication)
            //    throw new ForbiddenException();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                application.Accepted = applicationResponse;

                if (applicationResponse)
                {
                    var entity = await _context.QuizEditionResults.AddAsync(
                        new()
                        {
                            TeamId = application.TeamId,
                            EditionId = application.EditionId,
                        }
                    );

                    edition.AcceptedTeams += 1;

                    await _context.SaveChangesAsync();

                    var userIds = application.Users.Select(x => x.Id).ToList();

                    var sql = new StringBuilder("INSERT INTO user_edition (user_id, edition_result_id) VALUES ");
                    var parameters = new List<object>();

                    for (int i = 0; i < userIds.Count; i++)
                    {
                        sql.Append($"(@p{parameters.Count}, @p{parameters.Count + 1}),");

                        parameters.Add(userIds[i]);
                        parameters.Add(entity.Entity.Id);
                    }

                    sql.Length--;

                    await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
                }

                edition.PendingTeams -= 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw new DivineException();
            }
        }

        public async Task RemoveTeamFromEdition(int editionId, int teamId)
        {
            var edition = await GetById(editionId);

            if (edition.Time > DateTime.UtcNow)
                throw new BadRequestException("Can not remove a team before they do not show up!");

            var teamOnEdition = await _context.QuizEditionResults
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.TeamId == teamId && x.EditionId == editionId)
                ?? throw new NotFoundException($"Team {teamId} not found in edition {editionId}!");

            foreach (var user in teamOnEdition.Users)
                user.Rating -= 50;

            _context.QuizEditionResults.Remove(teamOnEdition);

            edition.AcceptedTeams -= 1;

            await _context.SaveChangesAsync();
        }

        public async Task WithdrawFromEdition(int editionId, int userId)
        {
            var edition = await GetById(editionId);

            if (edition.Time < DateTime.UtcNow)
                throw new BadRequestException("Can not withdraw after the edition started!");

            var application = await _context.QuizEditionApplications
                .FirstOrDefaultAsync(x => x.Users.Any(x => x.Id == userId) && x.EditionId == editionId && x.Accepted != false)
                ?? throw new NotFoundException($"Application for team with user {userId} in edition {editionId} not found!");

            var teamMember = await _context.UserTeams
                .FirstOrDefaultAsync(x => x.UserId == userId && x.TeamId == application.TeamId)
                ?? throw new ForbiddenException();

            if (!teamMember.RegisterTeam)
                throw new ForbiddenException();

            if (application.Accepted == null)
            {
                application.Accepted = false;
                edition.PendingTeams -= 1;
            }
            else if (application.Accepted == true)
            {
                // DODATI TABLICU TEAM_PULLED_OUT DA ORGANIZATORI ZNAJU JEL TIM UPORNO ODUSTAJE ?!?
                var teamOnEdition = await _context.QuizEditionResults
                .FirstOrDefaultAsync(x => x.TeamId == application.TeamId && x.EditionId == editionId)
                ?? throw new NotFoundException($"Team {application.TeamId} not found in edition {editionId}!");

                _context.QuizEditionResults.Remove(teamOnEdition);
                edition.AcceptedTeams -= 1;
                application.Accepted = false;
            }
            else
                throw new BadRequestException("Nisi prihvacen na izdanje, kako ces se odjaviti onda?");

                await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizEdition>> GetByLocationId(int locationId)
        {
            return await _context.QuizEditions
                .Include(x => x.Quiz)
                .Include(x => x.Category)
                .Include(x => x.Location).ThenInclude(l => l.City).ThenInclude(c => c.Country)
                .Where(x => x.LocationId == locationId)
                .ToListAsync();
        }

        private static void DateAndFeeCheck(NewQuizEditionDto editionDto, bool update = false)
        {
            if (!update)
            {
                if (editionDto.RegistrationEnd < DateTime.UtcNow)
                    throw new BadRequestException("You should let users apply!");

                if (editionDto.Time < DateTime.UtcNow)
                    throw new BadRequestException("Quiz already happened?");
            }

            if (editionDto.RegistrationEnd > editionDto.Time || editionDto.RegistrationStart > editionDto.Time || editionDto.RegistrationStart > editionDto.RegistrationEnd)
                throw new BadRequestException("Dates dont make sense!");

            if (editionDto.FeeType == 3 && editionDto.Fee != 0)
                throw new BadRequestException("Free quiz but there is a fee?");
        }

        private async Task<bool> UpdatePrizes(QuizEdition edition, NewQuizEditionDto editionDto)
        {
            bool changed = false;

            editionDto.Prizes ??= new List<PrizeDto>();

            var prizeIds = editionDto.Prizes.Select(p => p.Id).ToList();

            var toRemove = edition.EditionPrizes
                .Where(p => !prizeIds.Contains(p.Id))
                .ToList();
            
            if (toRemove.Count != 0)
                changed = true;

            foreach (var prize in toRemove)
                _context.EditionPrizes.Remove(prize);

            foreach (var prize in editionDto.Prizes)
            {
                var editionPrize = edition.EditionPrizes.FirstOrDefault(x => x.Id == prize.Id);

                if (editionPrize != null)
                {
                    if (editionPrize.Name != prize.Name || editionPrize.Position != prize.Position)
                    {
                        await _prizeRepository.UpdateEdition(prize);
                        changed = true;
                    }
                }
                else
                {
                    await _prizeRepository.AddEdition(prize, editionDto.Id);
                    changed = true;
                }
            }

            return changed;
        }

        private async Task<List<QuizRound>> GetRoundsWithAnswers(int editionId)
        {
            return await _context.QuizRounds
                .Include(x => x.QuizSegments)
                    .ThenInclude(s => s.QuizQuestions)
                .Where(x => x.EditionId == editionId)
                .ToListAsync();
        }

        private async Task<string> GetAltName(NewQuizEditionDto editionDto)
        {
            var baseName = editionDto.Name;
            var suffix = "#1";

            var match = Regex.Match(editionDto.Name, @"^(.*)\s+#(\d+)$");

            if (match.Success)
            {
                baseName = match.Groups[1].Value.Trim();
                var currentNumber = int.Parse(match.Groups[2].Value);
                suffix = $"#{currentNumber + 1}";
            }
            else
            {
                var count = await _context.QuizEditions
                    .Where(x => x.QuizId == editionDto.QuizId && x.Name.StartsWith(baseName))
                    .CountAsync();

                suffix = $"#{count + 1}";
            }

            return $"{baseName} {suffix}";
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool?> HasDetailedQuestions(int editionId)
        {
            return await _context.QuizEditions
                .Where(x => x.Id == editionId)
                .Select(x => x.DetailedQuestions)
                .FirstOrDefaultAsync();
        }

        public async Task SetDetailedQuestions(int editionId, int userId, bool detailed)
        {
            var edition = await _context.QuizEditions
                .FirstOrDefaultAsync(x => x.Id == editionId)
                ?? throw new NotFoundException("Edition not found!");

            if (edition.DetailedQuestions != null)
                throw new BadRequestException("Detailed questions already set!");

            edition.DetailedQuestions = detailed;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizEdition>> GetByTeamId(int teamId)
        {
            return await _context.QuizEditions
                .Include(x => x.Category)
                .Where(x => x.QuizEditionResults.Any(r => r.TeamId == teamId))
                .ToListAsync();
        }

        public async Task<QuizEditionRecommendationParamsDto> GetRecommendationInfoById(int id)
        {
            return await _context.QuizEditions
                .Where(x => x.Id == id)
                .Select(x => new QuizEditionRecommendationParamsDto
                {
                    EditionId = x.Id,
                    Rating = x.Rating,
                    RatingDifference = Math.Abs(x.Rating - (int)x.QuizEditionResults.SelectMany(r => r.Users).Average(u => u.Rating)),
                    Duration = x.Duration ?? 0,
                    CategoryId = x.CategoryId,
                    HostId = x.HostId,
                    NumberOfTeams = x.QuizEditionResults.Count,
                    TeamSize = (int)x.QuizEditionResults.Average(x => x.Users.Count),
                    TimeOfEdition = TimeOnly.FromDateTime(x.Time),
                    DayOfTheWeek = x.Time.DayOfWeek
                })
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Edition not found!");
        }
    }
}