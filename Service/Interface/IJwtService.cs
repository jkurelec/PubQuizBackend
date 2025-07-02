namespace PubQuizBackend.Service.Interface
{
    public interface IJwtService
    {
        string GenerateAccessToken(string userId, string username, int role, int? teamId, int app);
    }
}
