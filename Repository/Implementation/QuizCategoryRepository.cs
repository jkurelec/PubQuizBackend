using Microsoft.EntityFrameworkCore;
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

        public async Task<QuizCategoryDto?> Add(QuizCategoryDto quizCategory)
        {
            try
            {
                await _dbContext.QuizCategories.AddAsync(quizCategory.ToObject());
                await _dbContext.SaveChangesAsync();

                var newCategory = await GetById(quizCategory.Id);

                if (newCategory != null)
                {
                    QuizCategoryProvider.AddCategory(newCategory);

                    return newCategory;
                }

                return null;
            }
            catch
            {
                return null;
            }
            
        }

        public async Task<List<QuizCategoryDto>> GetAll()
        {
            return await _dbContext.QuizCategories.Select(x => new QuizCategoryDto(x)).ToListAsync();
        }

        public async Task<QuizCategoryDto?> GetById(int id)
        {
            var category = await _dbContext.QuizCategories.FindAsync(id);

            if(category != null)
                return new(category);

            return null;
        }
    }
}
