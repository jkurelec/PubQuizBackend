using PubQuizBackend.Enums;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.PrizesDto;
using System.ComponentModel.DataAnnotations;

namespace PubQuizBackend.Model.Dto.QuizEditionDto
{
    public class NewQuizEditionDto
    {
        public NewQuizEditionDto() { }

        public NewQuizEditionDto(QuizEdition quizEdition)
        {
            Id = quizEdition.Id;
            Name = quizEdition.Name;
            QuizId = quizEdition.QuizId;
            HostId = quizEdition.HostId;
            CategoryId = quizEdition.CategoryId;
            LocationId = quizEdition.LocationId;
            FeeType = quizEdition.FeeType;
            Fee = quizEdition.Fee;
            Duration = quizEdition.Duration;
            MaxTeamSize = quizEdition.MaxTeamSize;
            Description = quizEdition.Description;
            LeagueId = quizEdition.LeagueId;
            Time = quizEdition.Time;
            RegistrationStart = quizEdition.RegistrationStart;
            RegistrationEnd = quizEdition.RegistrationEnd;
        }

        public int Id { get; set; } = 0;
        public string Name { get; set; } = null!;
        public int QuizId { get; set; }
        public int HostId { get; set; }
        public int CategoryId { get; set; }
        public int LocationId { get; set; }
        public int Rating { get; set; }
        [Range(1, 3, ErrorMessage = "FeeType: 1 => Per Team, 2 => Per Member, 3 => Free")]
        public int? FeeType { get; set; }
        public int? Fee { get; set; }
        public int? Duration { get; set; }
        [Range(1, 6, ErrorMessage ="Team size should be between 1 and 6")]
        public int? MaxTeamSize { get; set; }
        public string? Description { get; set; }
        public int? LeagueId { get; set; }
        public IEnumerable<PrizeDto> Prizes { get; set; } = new List<PrizeDto>();
        public DateTime Time { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public int Visibility { get; set; } = 0;

        public QuizEdition ToObject() =>
            new()
            {
                Id = Id,
                Name = Name,
                QuizId = QuizId,
                HostId = HostId,
                CategoryId = CategoryId,
                LocationId = LocationId,
                Time = Time,
                Rating = Rating,
                FeeType = FeeType,
                Fee = Fee,
                Duration = Duration,
                MaxTeamSize = MaxTeamSize,
                Description = Description,
                LeagueId = LeagueId,
                RegistrationStart = RegistrationStart,
                RegistrationEnd = RegistrationEnd,
                Visibility = Visibility
            };
    }
}
