namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizAnswerRepository
    {
        Task IsVisible(int editionId, int userId);
    }
}
