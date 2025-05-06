using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.UserDto
{
    public class UserDto
    {
        public UserDto(){}

        public UserDto(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Firstname = user.Firstname;
            Lastname = user.Lastname;
            Email = user.Email;
            Rating = user.Rating;
            Role = user.Role;
        }

        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int Rating { get; set; }

        public int Role { get; set; }
    }
}
