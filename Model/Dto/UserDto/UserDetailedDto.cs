using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizAnswerDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Dto.TeamDto;

namespace PubQuizBackend.Model.Dto.UserDto
{
    public class UserDetailedDto
    {
        public UserDetailedDto() { }

        public UserDetailedDto(User user, IEnumerable<QuizMinimalDto> hostingQuizzes)
        {
            Id = user.Id;
            Username = user.Username;
            Rating = user.Rating;
            Role = user.Role;
            ProfileImage = user.ProfileImage!;
            HostingQuizzes = hostingQuizzes;
            EditionsHosted = user.QuizEditions.Select(x => new QuizEditionMinimalDto(x));
            CurrentTeams = user.Teams.Select(x => new TeamBreifDto(x));
            AttendedEditions = user.EditionResults.Select(x => new QuizEditionResultForUserDto(x));
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public int Rating { get; set; }
        public int Role { get; set; }
        public string ProfileImage { get; set; } = string.Empty;
        public IEnumerable<QuizMinimalDto> HostingQuizzes { get; set; } = new List<QuizMinimalDto>();
        public IEnumerable<QuizEditionMinimalDto> EditionsHosted { get; set; } = new List<QuizEditionMinimalDto>();
        public IEnumerable<TeamBreifDto> CurrentTeams { get; set; } = new List<TeamBreifDto>();
        public IEnumerable<QuizEditionResultForUserDto> AttendedEditions { get; set; } = new List<QuizEditionResultForUserDto>();
    }
}