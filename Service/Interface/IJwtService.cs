namespace PubQuizBackend.Service.Interface
{
    public interface IJwtService
    {
        public string GenerateAccessToken(string userId, string username, string role/*, int port*/);
    }
}
