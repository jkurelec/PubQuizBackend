using PubQuizBackend.Model.Other;

namespace PubQuizBackend.Repository.Interface
{
    public interface IQuestionMediaPermissionRepository
    {
        QuestionMediaPermissions GetPermissions();
        Task SetPermissions(IServiceProvider serviceProvider);
    }
}
