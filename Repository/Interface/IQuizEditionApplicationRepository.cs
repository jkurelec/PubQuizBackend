using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizEditionApplicationRepository
    {
        Task<IEnumerable<QuizEditionApplication>> GetByEditionId(int id, bool? accepted = null);
        Task<int> GetAcceptedCountByEditionId(int id);
        Task<int> GetMaxTeamsByEditionId(int id);
        Task<QuizEditionApplication> GetApplicationById(int id);
        Task<bool> CheckIfUserApplied(int userId, int editionId);
        Task<QuizEditionApplication> GetApplicationByUserAndEditionId(int userId, int editionId);
    }
}
