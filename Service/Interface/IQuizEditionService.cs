using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizEditionService
    {
        public Task<IEnumerable<QuizEditionBriefDto>> GetAll();
        public Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id);
        public Task<QuizEditionDetailedDto> GetById(int id);
        public Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId);
        public Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId);
        public Task<bool> Delete(int editionId, int userId);
    }
}
