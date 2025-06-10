using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IEloCalculatorRepository
    {
        Task<QuizEdition> GetEdition(int editionId);
        Task SaveChanges();
        Task AuthorizeHostByEditionId(int editionId, int hostId);
        Task<int> GetNumberOfParticipations(int id, bool team);
        Task<IEnumerable<int>> GetRatingsFromApplication(int applicationId);
        Task<Dictionary<int, Dictionary<int, List<QuestionResult>>>> GetAnswersForKappa();
    }
}
