using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public class QuizEditionRoundsDto
    {
        public QuizEditionRoundsDto() { }

        public QuizEditionRoundsDto(QuizEdition edition)
        {
            Id = edition.Id;
            Name = edition.Name;
            Rounds = edition.QuizRounds.Select(x => new QuizRoundDto(x)).ToList();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<QuizRoundDto> Rounds { get; set; } = null!;
    }
}
