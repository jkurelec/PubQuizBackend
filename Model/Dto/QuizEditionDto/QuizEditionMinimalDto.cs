using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public partial class QuizEditionMinimalDto
    {
        public QuizEditionMinimalDto() { }

        public QuizEditionMinimalDto(QuizEdition edition)
        {
            Id = edition.Id;
            Name = edition.Name;
            Category = new(edition.Category);
            Time = edition.Time;
            Rating = edition.Rating;
            MaxTeams = edition.MaxTeams;
            AcceptedTeams = edition.AcceptedTeams;
            PendingTeams = edition.PendingTeams;
            ProfileImage = edition.ProfileImage;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public QCategoryDto Category { get; set; } = null!;
        public DateTime Time { get; set; }
        public int Rating { get; set; }
        public int MaxTeams { get; set; }
        public int AcceptedTeams { get; set; }
        public int PendingTeams { get; set; }
        public string? ProfileImage { get; set; }
    }

    public partial class QuizEditionMinimalDto
    {
        public float? Match { get; set; }
    }
}
