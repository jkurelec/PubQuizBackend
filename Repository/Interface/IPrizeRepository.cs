using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface IPrizeRepository
    {
        public Task<EditionPrize> AddEdition(PrizeDto prizeDto, int parentId);
        public Task<LeaguePrize> AddLeague(PrizeDto prizeDto, int parentId);
        public Task<IEnumerable<PrizeDto>> GetByEditionId(int id);
        public Task<IEnumerable<PrizeDto>> GetByLeagueId(int id);
        public Task<PrizeDto> UpdateEdition(PrizeDto prizeDto);
        public Task<PrizeDto> UpdateLeague(PrizeDto prizeDto);
        public Task<bool> DeleteEdition(int id);
        public Task<bool> DeleteLeague(int id);
    }
}
