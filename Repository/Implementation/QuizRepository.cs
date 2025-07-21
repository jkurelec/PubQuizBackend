using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizRepository : IQuizRepository
    {
        private readonly PubQuizContext _context;

        public QuizRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task<Quiz> Add(NewQuizDto quizDto, int ownerId)
        {
            if (await _context.Quizzes.AnyAsync(x => x.Name == quizDto.Name))
                throw new ConflictException("Name already taken");

            await IsOwner(quizDto.OrganizationId, ownerId);

            var locations = await _context.Locations.Where(x => quizDto.Locations.Contains(x.Id)).ToListAsync();
            var categories = await _context.QuizCategories.Where(x => quizDto.Categories.Contains(x.Id)).ToListAsync();

            var quiz = quizDto.ToObject(categories, locations);
            quiz.ProfileImage = "default.jpg";

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Quizzes.AddAsync(quiz);
                await _context.SaveChangesAsync();

                quiz = await _context.Quizzes.FirstOrDefaultAsync(x => x.Name == quizDto.Name)
                    ?? throw new DivineException();

                await _context.HostOrganizationQuizzes.AddAsync(
                    new()
                    {
                        HostId = ownerId,
                        OrganizationId = quiz.OrganizationId,
                        QuizId = quiz.Id,
                        CreateEdition = true,
                        EditEdition = true,
                        DeleteEdition = true,
                        CrudQuestion = true,
                        ManageApplication = true
                    }
                );

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return await _context.Quizzes
                .Include(x => x.Organization).ThenInclude(o => o.Owner).ThenInclude(o => o.HostOrganizationQuizzes)
                .Include(x => x.QuizCategories)
                .Include(x => x.Locations).ThenInclude(l => l.City).ThenInclude(c => c.Country)
                .Include(x => x.QuizEditions).ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(x => x.Name == quizDto.Name)
                ?? throw new DivineException();
        }

        public async Task<bool> Delete(int id, int ownerId)
        {
            await IsOwner(id, ownerId);

            var quiz = await _context.Quizzes.FindAsync(id)
                ?? throw new ForbiddenException();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Quiz>> GetAll()
        {
            return await _context.Quizzes.Include(x => x.Organization).ToListAsync();
        }

        public async Task<Quiz> GetById(int id)
        {
            return await _context.Quizzes.Include(x => x.Organization).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("Quiz not found!");
        }

        public async Task<IEnumerable<Quiz>> GetAllDetailed()
        {
            return await _context.Quizzes.Include(x => x.QuizCategories).Include(x => x.Locations).ThenInclude(l => l.City).ThenInclude(c => c.Country).Include(x => x.QuizEditions).ThenInclude(e => e.Category).Include(x => x.Teams).Include(x => x.Organization).ToListAsync();
        }

        public async Task<Quiz> GetByIdDetailed(int id)
        {
            return await _context.Quizzes.Include(x => x.QuizCategories).Include(x => x.Locations).ThenInclude(l => l.City).ThenInclude(c => c.Country).Include(x => x.QuizEditions).ThenInclude(e => e.Category).Include(x => x.Teams).Include(x => x.Organization).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new NotFoundException("Quiz not found!");
        }

        public async Task<Quiz> Update(NewQuizDto quizDto, int ownerId)
        {
            await IsOwner(quizDto.OrganizationId, ownerId);

            var quiz = await _context.Quizzes.Include(x => x.QuizCategories).Include(x => x.Locations).ThenInclude(l => l.City).ThenInclude(c => c.Country).Include(x => x.QuizEditions).ThenInclude(e => e.Category).Include(x => x.Teams).Include(x => x.Organization).FirstOrDefaultAsync(x => x.Id == quizDto.Id)
                ?? throw new DivineException();

            if (quiz.OrganizationId != quizDto.OrganizationId)
                throw new ForbiddenException();

            if (quizDto.Name == null || quizDto.Name == "" || quizDto.Locations.Count == 0 || quizDto.Locations == null || quizDto.Categories.Count == 0 || quizDto.Categories == null)
                throw new BadRequestException("None of the properties can be empty!");

            var quizLocationIds = quiz.Locations.Select(l => l.Id).ToHashSet();
            var providedLocationIds = quizDto.Locations.ToHashSet();
            var quizCategoryIds = quiz.Locations.Select(l => l.Id).ToHashSet();
            var providedCategoryIds = quizDto.Categories.ToHashSet();

            if (quiz.Name != quizDto.Name)
                quiz.Name = quizDto.Name;

            if (!quizLocationIds.SetEquals(providedLocationIds))
                quiz.Locations = await _context.Locations.Where(x => quizDto.Locations.Contains(x.Id)).ToListAsync();

            if (!quizCategoryIds.SetEquals(providedCategoryIds))
                quiz.QuizCategories = await _context.QuizCategories.Where(x => quizDto.Categories.Contains(x.Id)).ToListAsync();

            _context.Entry(quiz).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return quiz;
        }

        public async Task<IEnumerable<Quiz>> GetByHostAndOrganization(int hostId, int organizationId)
        {
            return await _context.HostOrganizationQuizzes
                .Include(x => x.Quiz)
                .Where(x =>
                    x.HostId == hostId
                    && x.OrganizationId == organizationId
                )
                .Select(x => x.Quiz)
                .ToListAsync();
        }

        public async Task<Quiz> GetOwnerQuizByIds(int ownerId, int quizId)
        {
            return await _context.Quizzes
                .Include(x => x.Organization)
                .Where(x => x.Organization.OwnerId == ownerId && x.Id == quizId)
                .FirstOrDefaultAsync()
                ?? throw new ForbiddenException();
        }

        // OVO MOZE BITI ANYASYNC
        private async Task IsOwner(int organizerId, int ownerId)
        {
            _ = await _context.Organizations.FirstOrDefaultAsync(x => x.OwnerId == ownerId && x.Id == organizerId)
                ?? throw new ForbiddenException();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
