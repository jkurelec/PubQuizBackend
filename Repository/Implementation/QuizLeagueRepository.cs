using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using PubQuizBackend.Model.Dto.QuizLeagueDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;
using System.Text.RegularExpressions;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizLeagueRepository : IQuizLeagueRepository
    {
        private readonly PubQuizContext _context;
        private readonly IPrizeRepository _prizeRepository;
        private readonly string PointsRegex = @"^(\d+=\d+)(\|\d+=\d+)*$";

        public QuizLeagueRepository(PubQuizContext context, IPrizeRepository prizeRepository)
        {
            _context = context;
            _prizeRepository = prizeRepository;
        }

        public async Task<QuizLeague> Add(NewQuizLeagueDto leagueDto, int userId)
        {
            var owner = await _context.Quizzes.FirstOrDefaultAsync(x => x.Organization.OwnerId == userId && x.Id == leagueDto.QuizId)
                ?? throw new UnauthorizedException();

            if (await _context.QuizLeagues.AnyAsync(x => x.Name == leagueDto.Name))
                throw new BadRequestException("League name already exists!");

            if (!Regex.IsMatch(leagueDto.Points, PointsRegex))
                throw new BadRequestException("Points not matching the pattern!");

            var prizes = new List<LeaguePrize>();

            var entity = await _context.QuizLeagues.AddAsync(leagueDto.ToObject());
            await _context.SaveChangesAsync();

            foreach (var prizeDto in leagueDto.Prizes)
                prizes.Add(await _prizeRepository.AddLeague(prizeDto, entity.Entity.Id));

            return await GetById(entity.Entity.Id);
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var league = await _context.QuizLeagues.Include(x => x.Quiz).ThenInclude(q => q.Organization).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("League not found!");

            if (league.Quiz.Organization.OwnerId != userId)
                throw new UnauthorizedException();

            _context.QuizLeagues.Remove(league);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<QuizLeague> GetById(int id)
        {
            return await _context.QuizLeagues
                .Include(x => x.LeaguePrizes)
                .Include(x => x.Quiz)
                .Include(x => x.QuizEditions)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("League not found!");
        }

        public async Task<IEnumerable<QuizLeague>> GetByQuizId(int id)
        {
            var leagues = await _context.QuizLeagues.Where(x => x.QuizId == id).Include(x => x.Quiz).ToListAsync();

            return leagues.Count == 0
                ? throw new NotFoundException("No leagues found!")
                : leagues;
        }

        public async Task<QuizLeague> Update(NewQuizLeagueDto leagueDto, int userId)
        {
            var league = await _context.QuizLeagues.Include(x => x.Quiz).Include(x => x.LeaguePrizes).FirstOrDefaultAsync(x => x.Id == leagueDto.Id)
                ?? throw new NotFoundException("League not found!");

            if (league.Quiz.Organization.OwnerId != userId)
                throw new UnauthorizedException();

            PropertyUpdater.UpdateEntityFromDto(league, leagueDto, ["Id", "QuizId"]);

            if (leagueDto.Prizes == null)
                leagueDto.Prizes = new List<PrizeDto>();

            var prizeIds = leagueDto.Prizes.Select(p => p.Id).ToList();

            var toRemove = league.LeaguePrizes
                .Where(p => !prizeIds.Contains(p.Id))
                .ToList();

            foreach (var prize in toRemove)
                _context.LeaguePrizes.Remove(prize);

            foreach (var prize in leagueDto.Prizes)
            {
                var leaguePrize = league.LeaguePrizes.FirstOrDefault(x => x.Id == prize.Id);

                if (leaguePrize != null)
                {
                    if (leaguePrize.Name != prize.Name || leaguePrize.Position != prize.Position)
                        await _prizeRepository.UpdateLeague(prize);
                }
                else
                    await _prizeRepository.AddLeague(prize, leagueDto.Id);
            }
            
            await _context.SaveChangesAsync();

            return await GetById(leagueDto.Id);
        }
    }
}
