using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuizEditionService
    {
        Task<IEnumerable<QuizEditionBriefDto>> GetAll();
        Task<IEnumerable<QuizEditionBriefDto>> GetByQuizId(int id);
        Task<QuizEditionDetailedDto> GetById(int id);
        Task<QuizEditionDetailedDto> Add(NewQuizEditionDto editionDto, int userId);
        Task<QuizEditionDetailedDto> Update(NewQuizEditionDto editionDto, int userId);
        Task Delete(int editionId, int userId);
        Task ApplyTeam(QuizEditionApplicationRequestDto application, int registrantId);
        Task<IEnumerable<QuizEditionApplicationDto>> GetApplications(int editionId, int hostId, bool unanswered);
        Task RespondToApplication(QuizEditionApplicationResponseDto application, int hostId);
    }
}
