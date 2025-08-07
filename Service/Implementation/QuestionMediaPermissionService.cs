using PubQuizBackend.Model.Other;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class QuestionMediaPermissionService : IQuestionMediaPermissionService
    {
        private readonly IQuestionMediaPermissionRepository _repository;

        public QuestionMediaPermissionService(IQuestionMediaPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<QuestionMediaPermissions> GetPermissions()
        {
            return await _repository.GetPermissions();
        }
    }
}
