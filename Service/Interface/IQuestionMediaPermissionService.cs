using PubQuizBackend.Model.Other;

namespace PubQuizBackend.Service.Interface
{
    public interface IQuestionMediaPermissionService
    {
        QuestionMediaPermissions GetPermissions();
        Task SetPermissions(IServiceProvider serviceProvider);
    }
}
