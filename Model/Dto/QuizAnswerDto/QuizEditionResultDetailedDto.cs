using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.QuizAnswerDto
{
    public class QuizEditionResultDetailedDto : QuizEditionResultBriefDto
    {
        public QuizEditionResultDetailedDto() { }

        public QuizEditionResultDetailedDto(QuizEditionResult editionResult) : base(editionResult)
        {
            Rounds = editionResult.QuizRoundResults.Select(x => new QuizRoundResultDetailedDto(x)).ToList();
        }
        public new IEnumerable<QuizRoundResultDetailedDto> Rounds { get; set; } = new List<QuizRoundResultDetailedDto>();
    }
}
