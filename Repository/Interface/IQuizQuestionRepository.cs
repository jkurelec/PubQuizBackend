namespace PubQuizBackend.Repository.Interface
{
    public interface IQuizQuestionRepository
    {
        void IsVisible(int editionId, int user);
    }
}
