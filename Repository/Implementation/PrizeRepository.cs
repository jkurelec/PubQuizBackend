using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    //jel treba za van update jer samo lupa save async a rijetko kad je jedan
    public class PrizeRepository : IPrizeRepository
    {
        private readonly PubQuizContext _context;

        public PrizeRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<EditionPrize> AddEdition(PrizeDto prizeDto, int parentId)
        {
            var prize = await _context.EditionPrizes.AddAsync(prizeDto.ToEdition(parentId));
            await _context.SaveChangesAsync();

            return prize.Entity;
        }

        public async Task<LeaguePrize> AddLeague(PrizeDto prizeDto, int parentId)
        {
            var prize = await _context.LeaguePrizes.AddAsync(prizeDto.ToLeague(parentId));
            await _context.SaveChangesAsync();

            return prize.Entity;
        }

        public async Task<bool> DeleteEdition(int id)
        {
            var prize = await _context.EditionPrizes.FindAsync(id)
                ?? throw new NotFoundException("Prize not found!");

            _context.EditionPrizes.Remove(prize);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteLeague(int id)
        {
            var prize = await _context.LeaguePrizes.FindAsync(id)
                ?? throw new NotFoundException("Prize not found!");

            _context.LeaguePrizes.Remove(prize);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PrizeDto>> GetByEditionId(int id)
        {
            return await _context.EditionPrizes
                .Where(x => x.EditionId == id)
                .Select(x => new PrizeDto(x))
                .ToListAsync();
        }

        public async Task<IEnumerable<PrizeDto>> GetByLeagueId(int id)
        {
            return await _context.LeaguePrizes
                .Where(x => x.LeagueId == id)
                .Select(x => new PrizeDto(x))
                .ToListAsync();
        }

        public async Task<PrizeDto> UpdateEdition(PrizeDto prizeDto)
        {
            var prize = await _context.EditionPrizes.FindAsync(prizeDto.Id)
                ?? throw new NotFoundException("Prize not found!");

            PropertyUpdater.UpdateEntityFromDto(prize, prizeDto, ["id"]);

            await _context.SaveChangesAsync();

            return new(prize);
        }

        public async Task<PrizeDto> UpdateLeague(PrizeDto prizeDto)
        {
            var prize = await _context.LeaguePrizes.FindAsync(prizeDto.Id)
                ?? throw new NotFoundException("Prize not found!");

            PropertyUpdater.UpdateEntityFromDto(prize, prizeDto, ["Id"]);

            await _context.SaveChangesAsync();

            return new(prize);
        }
    }
}
