using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizEditionRepository : IQuizEditionRepository
    {
        private readonly PubQuizContext _context;

        public QuizEditionRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<QuizEdition> Add(NewQuizEditionDto editionDto, int userId)
        {
            var host = await _context.HostOrganizationQuizzes.Where(x => x.HostId == userId && x.QuizId == editionDto.QuizId).FirstOrDefaultAsync()
                ?? throw new UnauthorizedException();

            if (!host.CreateEdition)
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

            //ako unose stare kvizove?
            if (editionDto.Time < DateTime.Now)
                throw new BadRequestException("Quiz already happened?");

            if (editionDto.RegistrationEnd > editionDto.Time || editionDto.RegistrationStart > editionDto.Time || editionDto.RegistrationStart > editionDto.RegistrationEnd)
                throw new BadRequestException("Dates dont make sense!");

            if (editionDto.FeeType == 3 && editionDto.Fee != 0)
                throw new BadRequestException("Free quiz but there is a fee?");
            
            var quiz = await _context.Quizzes.FindAsync(editionDto.QuizId)
                ?? throw new TotalnoSiToPromislioException();

            editionDto.Rating = quiz.Rating;

            var entity = await _context.QuizEditions.AddAsync(editionDto.ToObject());
            await _context.SaveChangesAsync();

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
            throw new NotImplementedException();
        }
    }
}
