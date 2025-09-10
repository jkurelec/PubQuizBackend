using PubQuizBackend.Model;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class BaseRepository : IBaseRepository
    {
        private readonly PubQuizContext _context;

        public BaseRepository(PubQuizContext context)
        {
            _context = context;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
