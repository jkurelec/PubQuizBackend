using System.ComponentModel.DataAnnotations;

namespace PubQuizBackend.Models.Dto
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(15, ErrorMessage = "Username mora biti izmedu 4-15 znakova", MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Username može sadržavati samo slova.")]
        public string Username { get; set; } = null!;

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$", 
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;

        [Required]
        [Range(1,4, ErrorMessage = "Role mora biti izmedu 1-4")]
        public int Role { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Username mora biti izmedu 4-15 znakova", MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Username može sadržavati samo slova.")]
        public string Firstname { get; set; } = null!;

        [Required]
        [StringLength(15, ErrorMessage = "Username mora biti izmedu 4-15 znakova", MinimumLength = 4)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Username može sadržavati samo slova.")]
        public string Lastname { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
