namespace PubQuizBackend.Repository.Interface
{
    public interface IHostRepository
    {
        Task<IEnumerable<int>> GetQuizIdsByHostAndOrganization(int hostId, int organizationId);
    }
}
