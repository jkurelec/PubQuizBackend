using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizAnswerDto;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public class PastQuizEditionDetailedDto : QuizEditionDetailedDto
    {
        public PastQuizEditionDetailedDto() { }

        public PastQuizEditionDetailedDto(QuizEdition edition) : base(edition)
        {
            DetailedQuestions = edition.DetailedQuestions;
            Rated = edition.Rated;
            Rounds = new(edition);
            Results = edition.QuizEditionResults.Select(x => new QuizEditionResultDetailedDto(x))
                .ToList();
        }
        
        public bool? DetailedQuestions { get; set; } = null;
        public bool Rated { get; set; }
        public QuizEditionRoundsDto Rounds { get; set; } = new();
        public IEnumerable<QuizEditionResultDetailedDto> Results { get; set; } = new List<QuizEditionResultDetailedDto>();
    }
}
