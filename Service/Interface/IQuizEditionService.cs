using PubQuizBackend.Enums;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizEditionService
    {
        Task<IEnumerable<QuizEditionBriefDto>> GetAll();
        Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id);
        Task<QuizEditionDetailedDto> GetById(int id, int? user = null);
        Task<IEnumerable<QuizEditionMinimalDto>> GetPage(int page, int pageSize, EditionFilter editionFilter);
        Task<IEnumerable<QuizEditionMinimalDto>> GetUpcomingCompletedPage(int page, int pageSize, EditionFilter editionFilter, bool upcoming = true);
        Task<int> GetTotalCount(EditionTimeFilter filter);
        Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId);
        Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId);
        Task Delete(int editionId, int userId);
        Task<IEnumerable<QuizEditionBriefDto>> GetByLocationId(int locationId);
        Task<string> UpdateProfileImage(IFormFile image, int editionId, int hostId);
        Task<bool?> HasDetailedQuestions(int editionId);
        Task SetDetailedQuestions(int editionId, int userId, bool detailed);
    }
}
