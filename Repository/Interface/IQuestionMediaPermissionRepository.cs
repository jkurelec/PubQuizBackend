using PubQuizBackend.Model.Other;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuestionMediaPermissionRepository
    {
        Task<QuestionMediaPermissions> GetPermissions();
    }
}
