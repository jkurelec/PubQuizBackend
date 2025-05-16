using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.QuizLeagueDto;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public class QuizEditionDetailedDto
    {
        public QuizEditionDetailedDto() { }

        public QuizEditionDetailedDto(QuizEdition edition)
        {
            Id = edition.Id;
            Name = edition.Name;
            Quiz = new(edition.Quiz);
            Host = new(edition.Host);
            Category = new(edition.Category);
            Location = new(edition.Location);
            Time = edition.Time;
            Rating = edition.Rating;
            AveragePoints = edition.QuizEditionResults.Count != 0
                ? edition.QuizEditionResults?.Average(x => x.TotalPoints)
                : 0;
            TotalPoints = edition.TotalPoints;
            FeeType = edition.FeeType;
            Fee = edition.Fee;
            Duration = edition.Duration;
            MaxTeamSize = edition.MaxTeamSize;
            Description = edition.Description;
            RegistrationStart = edition.RegistrationStart;
            RegistrationEnd = edition.RegistrationEnd;
            if (edition.League != null)
                League = new(edition.League);
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public QuizMinimalDto Quiz { get; set; } = null!;
        public UserBriefDto Host { get; set; } = null!;
        public QCategoryDto Category { get; set; } = null!;
        public LocationBriefDto Location { get; set; } = null!;
        public DateTime Time { get; set; }
        public int Rating { get; set; }
        public decimal? AveragePoints { get; set; }
        public decimal? TotalPoints { get; set; }
        public int? FeeType { get; set; }
        public int? Fee { get; set; }
        public int? Duration { get; set; }
        public int? MaxTeamSize { get; set; }
        public string? Description { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public QuizLeagueBriefDto? League { get; set; }
    }
}
