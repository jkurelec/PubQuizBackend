using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IPrizeRepository
    {
        Task<EditionPrize> AddEdition(PrizeDto prizeDto, int parentId);
        Task<LeaguePrize> AddLeague(PrizeDto prizeDto, int parentId);
        Task<IEnumerable<PrizeDto>> GetByEditionId(int id);
        Task<IEnumerable<PrizeDto>> GetByLeagueId(int id);
        Task<PrizeDto> UpdateEdition(PrizeDto prizeDto);
        Task<PrizeDto> UpdateLeague(PrizeDto prizeDto);
        Task<bool> DeleteEdition(int id);
        Task<bool> DeleteLeague(int id);
    }
}
