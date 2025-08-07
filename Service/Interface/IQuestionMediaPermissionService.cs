using PubQuizBackend.Model.Other;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuestionMediaPermissionService
    {
        Task<QuestionMediaPermissions> GetPermissions();
    }
}
