using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.PrizesDto
{
    public class PrizeDto
    {
        public PrizeDto() { }

        public PrizeDto(EditionPrize prize)
        {
            Id = prize.Id;
            ParentId = prize.EditionId;
            Name = prize.Name;
            Position = prize.Position;
        }

        public PrizeDto(LeaguePrize prize)
        {
            Id = prize.Id;
            ParentId = prize.LeagueId;
            Name = prize.Name;
            Position = prize.Position;
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; } = null!;
        public int? Position { get; set; }

        public EditionPrize ToEdition(int parentId)
        {
            return new()
            {
                Id = Id,
                EditionId = parentId,
                Name = Name,
                Position = Position
            };
        }

        public LeaguePrize ToLeague(int parentId)
        {
            return new()
            {
                Id = Id,
                LeagueId = parentId,
                Name = Name,
                Position = Position
            };
        }
    }
}
