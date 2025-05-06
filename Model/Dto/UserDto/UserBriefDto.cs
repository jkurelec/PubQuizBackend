using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.UserDto
{
    public class UserBriefDto
    {
        public UserBriefDto() {}

        public UserBriefDto(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            Rating = user.Rating;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
    }
}
