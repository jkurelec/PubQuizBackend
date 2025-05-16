using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Model.Dto.QuizDto;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public class QuizEditionBriefDto
    {
        public QuizEditionBriefDto() { }

        public QuizEditionBriefDto(QuizEdition edition)
        {
            Id = edition.Id;
            Name = edition.Name;
            Quiz = new(edition.Quiz);
            Category = new(edition.Category);
            Location = new(edition.Location);
            Time = edition.Time;
            Rating = edition.Rating;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public QuizMinimalDto Quiz { get; set; } = null!;
        public QCategoryDto Category { get; set; } = null!;
        public LocationBriefDto Location { get; set; } = null!;
        public DateTime Time { get; set; }
        public int Rating { get; set; }
    }
}
