using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizAnswerDto;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public class PastQuizEditionDetailedDto : QuizEditionDetailedDto
    {
        public PastQuizEditionDetailedDto() { }

        public PastQuizEditionDetailedDto(QuizEdition edition) : base(edition)
        {
            Rounds = new(edition);
            Results = edition.QuizEditionResults.Select(x => new QuizEditionResultDetailedDto(x))
                .ToList();
        }

        public QuizEditionRoundsDto Rounds { get; set; } = new();
        public IEnumerable<QuizEditionResultDetailedDto> Results { get; set; } = new List<QuizEditionResultDetailedDto>();
    }
}
