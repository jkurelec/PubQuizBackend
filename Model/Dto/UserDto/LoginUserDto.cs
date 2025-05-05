using System.ComponentModel.DataAnnotations;

namespace PubQuizBackend.Model.Dto.UserDto
{
    public class LoginUserDto/* : IValidatableObject*/
    {
        [Required(ErrorMessage = "Username or email is required.")]
        public string Identifier { get; set; } = null!;

        [Required]
        //[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$",
        //    ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = null!;

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (string.IsNullOrWhiteSpace(Identifier))
        //    {
        //        yield return new ValidationResult("Identifier is required.", new[] { nameof(Identifier) });
        //        yield break;
        //    }

        //    bool isEmail = new EmailAddressAttribute().IsValid(Identifier);
        //    bool isUsername = Regex.IsMatch(Identifier, @"^[a-zA-Z]{4,15}$");

        //    if (!isEmail && !isUsername)
        //    {
        //        yield return new ValidationResult(
        //            "Identifier must be a valid email or a username (4–15 letters, no numbers or symbols).",
        //            new[] { nameof(Identifier) });
        //    }
        //}
    }
}
