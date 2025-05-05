namespace PubQuizBackend.Model.Dto.UserDto
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int Rating { get; set; }

        public int Role { get; set; }
    }
}
