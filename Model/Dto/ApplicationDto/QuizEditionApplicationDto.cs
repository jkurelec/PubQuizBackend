using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Model.Dto.UserDto;

namespace PubQuizBackend.Model.Dto.ApplicationDto
{
    public class QuizEditionApplicationDto
    {
        public QuizEditionApplicationDto(QuizEditionApplication application)
        {
            Id = application.Id;
            TeamId = application.Team.Id;
            TeamName = application.Team.Name;
            TeamCategory = new(application.Team.Category);
            TeamQuiz = new(application.Team.Quiz);
            TeamMembers = application.Users.Select(x => new UserBriefDto(x)).ToList();
            Response = application.Accepted;
        }

        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = null!;
        public bool? Response { get; set; }
        public QCategoryDto TeamCategory { get; set; } = null!;
        public QuizMinimalDto TeamQuiz { get; set; } = null!;
        public IEnumerable<UserBriefDto> TeamMembers { get; set; } = new List<UserBriefDto>();
    }
}
