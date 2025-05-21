using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Repository.Implementation
{
    public class QuizCategoryRepository : IQuizCategoryRepository
    {
        private readonly PubQuizContext _dbContext;

        public QuizCategoryRepository(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QCategoryDto> Add(QCategoryDto quizCategory)
        {
            if (await _dbContext.QuizCategories.AnyAsync(x => x.Name == quizCategory.Name))
                throw new ConflictException("Category already exists!");

            quizCategory.Id = 0;

            await _dbContext.QuizCategories.AddAsync(quizCategory.ToObject());
            await _dbContext.SaveChangesAsync();

            var newCategory = await _dbContext.QuizCategories.FirstOrDefaultAsync(x => x.Name == quizCategory.Name)
                ?? throw new DivineException();

            var newCategoryDto = new QCategoryDto(newCategory);
            QuizCategoryProvider.AddCategory(newCategoryDto);

            return newCategoryDto;
        }

        public async Task<List<QCategoryDto>> GetAll()
        {
            return await _dbContext.QuizCategories.Select(x => new QCategoryDto(x)).ToListAsync();
        }

        public async Task<QCategoryDto> GetById(int id)
        {
            return new(
                await _dbContext.QuizCategories.FindAsync(id)
                    ?? throw new NotFoundException("Category not found!")
            );
        }
    }
}
