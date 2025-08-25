namespace PubQuizBackend.Service.Interface
{
    public interface IEloCalculatorService
    {
        Task CalculateEditionElo(int editionId, int hostId);
        Task<int> GetTeamInitialRating(int applicationId);
        Task CalculateKappa(CancellationToken cancellationToken = default);
        Task GetTeamKappas(CancellationToken cancellationToken = default);
        Task GetDeviations(CancellationToken cancellationToken = default);
        Task<bool> IsEditionRated(int editionId);
        Task<Dictionary<string, float>> GetProbability(int editionId);
    }
}
