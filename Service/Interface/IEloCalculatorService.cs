using System;

namespace PubQuizBackend.Service.Interface
{
    public interface IEloCalculatorService
    {
        Task CalculateEditionElo(int editionId, int hostId);
        Task<int> GetTeamInitialRating(int applicationId);
        Task CalculateKappa(CancellationToken cancellationToken = default);
        Task<bool> IsEditionRated(int editionId);
    }
}
