using PubQuizBackend.Model.Dto.QuizDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizService
    {
        Task<QuizDetailedDto> Add(NewQuizDto quizDto, int hostId);
        Task<object> GetById(int id, bool detailed = false);
        Task<IEnumerable<object>> GetAll(bool detailed = false);
        Task<QuizDetailedDto> Update(NewQuizDto quizDto, int hostId);
        Task<string> UpdateProfileImage(IFormFile image, int quizId, int hostId);
        Task<bool> Delete(int id, int hostId);
        Task<IEnumerable<QuizMinimalDto>> GetByHostAndOrganization(int hostId, int organizationId);
    }
}
