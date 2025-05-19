using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizEditionRepository : IQuizEditionRepository
    {
        private readonly PubQuizContext _context;
        private readonly IPrizeRepository _prizeRepository;

        public QuizEditionRepository(PubQuizContext context, IPrizeRepository prizeRepository)
        {
            _context = context;
            _prizeRepository = prizeRepository;
        }

        public async Task<QuizEdition> Add(NewQuizEditionDto editionDto, int userId)
        {
            var organizer = await _context.HostOrganizationQuizzes.Where(x => x.HostId == userId && x.QuizId == editionDto.QuizId).FirstOrDefaultAsync()
                ?? throw new UnauthorizedException();

            if (!organizer.CreateEdition)
                throw new UnauthorizedException();

            if (editionDto.LeagueId != null)
            {
                var league = await _context.QuizLeagues.FindAsync(editionDto.LeagueId)
                    ?? throw new NotFoundException("League not found");

                if (league.QuizId != editionDto.QuizId)
                    throw new UnauthorizedException();
            }

            if (userId != editionDto.HostId)
                if (!await _context.HostOrganizationQuizzes.AnyAsync(x => x.HostId == editionDto.HostId && x.QuizId == editionDto.QuizId))
                    throw new BadRequestException("Who is the host?");

            if (await _context.QuizEditions.AnyAsync(x => x.QuizId == editionDto.QuizId && x.Name == editionDto.Name))
            {
                var altName =
                    editionDto.Name
                    + " #"
                    + await _context.QuizEditions.Where(
                        x => x.QuizId == editionDto.QuizId && x.Name.Contains(editionDto.Name)
                        ).CountAsync()
                        .ContinueWith(x => x.Result + 1);

                throw new ConflictException($"Name already taken! Suggestion: {altName}");
            }

            if (!await _context.QuizCategories.AnyAsync(x => x.Id == editionDto.CategoryId))
                throw new BadRequestException("Category?");

            if (!await _context.Locations.AnyAsync(x => x.Id == editionDto.LocationId))
                throw new BadRequestException("Location?");

            DateAndFeeCheck(editionDto);

            var quiz = await _context.Quizzes.FindAsync(editionDto.QuizId)
                ?? throw new TotalnoSiToPromislioException();

            editionDto.Rating = quiz.Rating;

            editionDto.Id = 0;

            var prizes = new List<EditionPrize>();

            var entity = await _context.QuizEditions.AddAsync(editionDto.ToObject());
            await _context.SaveChangesAsync();

            foreach (var prizeDto in editionDto.Prizes)
                prizes.Add(await _prizeRepository.AddEdition(prizeDto, entity.Entity.Id));

            return await GetById(entity.Entity.Id);
        }

        public async Task<bool> Delete(int editionId, int userId)
        {
            var edition = await _context.QuizEditions.FirstOrDefaultAsync(x => x.Id == editionId)
                ?? throw new UnauthorizedException();

            var host = await _context.HostOrganizationQuizzes.Where(x => x.HostId == userId && x.QuizId == edition.QuizId).FirstOrDefaultAsync()
                ?? throw new UnauthorizedException();

            if (!host.DeleteEdition)
                throw new UnauthorizedException();

            _context.QuizEditions.Remove(edition);
            await _context.SaveChangesAsync();

            return true;
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
            return await _context.QuizEditions
                .Include(x => x.Host)
                .Include(x => x.Category)
                .Include(x => x.Location)
                .ThenInclude(l => l.City)
                .ThenInclude(c => c.Country)
                .Include(x => x.Quiz)
                .Include(x => x.League)
                .Include(x => x.EditionPrizes)
                .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Edition not found!");
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

        public async Task<QuizEdition> Update(NewQuizEditionDto editionDto, int userId)
        {
            var organizer = await _context.HostOrganizationQuizzes.Where(x => x.HostId == userId && x.QuizId == editionDto.QuizId).FirstOrDefaultAsync()
                ?? throw new UnauthorizedException();

            if (!organizer.EditEdition)
                throw new UnauthorizedException();

            var edition = await _context.QuizEditions.Include(x => x.EditionPrizes).FirstOrDefaultAsync(x => x.Id == editionDto.Id && x.QuizId == editionDto.QuizId)
                ?? throw new BadRequestException();

            if (edition.Name != editionDto.Name)
            {
                var nameTaken = await _context.QuizEditions.AnyAsync(x => x.Name == editionDto.Name && x.QuizId == editionDto.QuizId);

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
                var validLocation = await _context.QuizLeagues.AnyAsync(x => x.Id == editionDto.LeagueId);

                if (validLocation)
                    edition.LeagueId = editionDto.LeagueId;
                else
                    throw new BadRequestException("Invalid league!");
            }

            DateAndFeeCheck(editionDto);

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

            return await GetById(edition.Id);
        }

        private static void DateAndFeeCheck(NewQuizEditionDto editionDto)
        {
            //ako unose stare kvizove?
            if (editionDto.Time < DateTime.Now)
                throw new BadRequestException("Quiz already happened?");

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
    }
}