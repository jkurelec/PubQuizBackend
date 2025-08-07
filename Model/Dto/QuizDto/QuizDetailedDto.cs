using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Dto.QuizLeagueDto;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Model.Dto.QuizDto
{
    public class QuizDetailedDto
    {
        public QuizDetailedDto(Quiz quiz)
        {
            Id = quiz.Id;
            Name = quiz.Name;
            Organization = new(quiz.Organization);
            Rating = quiz.Rating;
            EditionsHosted = quiz.EditionsHosted;
            Locations = quiz.Locations.Select(x => new LocationBriefDto(x)).ToList();
            Categories = quiz.QuizCategories.Select(x => new QCategoryDto(x)).ToList();
            QuizLeagues = quiz.QuizLeagues.Select(x => new QuizLeagueMinimalDto(x)).ToList();
            QuizEditions = quiz.QuizEditions.Select(x => new QuizEditionMinimalDto(x));
            Teams = quiz.Teams.Select(x => new TeamBreifDto(x));
            ProfileImage = quiz.ProfileImage;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public OrganizationMinimalDto Organization { get; set; } = new();
        public int Rating { get; set; }
        public int EditionsHosted { get; set; }
        public IEnumerable<LocationBriefDto> Locations { get; set; } = new List<LocationBriefDto>();
        public IEnumerable<QCategoryDto> Categories { get; set; } = new List<QCategoryDto>();
        public IEnumerable<QuizLeagueMinimalDto> QuizLeagues { get; set; } = null!;
        public IEnumerable<QuizEditionMinimalDto> QuizEditions { get; set; } = null!;
        public IEnumerable<TeamBreifDto> Teams { get; set; } = null!;
        public string? ProfileImage { get; set; }
    }
}
