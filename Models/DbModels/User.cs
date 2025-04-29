using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PubQuizBackend.Models.DbModels;

[Table("user")]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username", TypeName = "character varying")]
    public string Username { get; set; } = null!;

    [Column("password", TypeName = "character varying")]
    public string Password { get; set; } = null!;

    [Column("role")]
    public int Role { get; set; }

    [Column("firstname", TypeName = "character varying")]
    public string Firstname { get; set; } = null!;

    [Column("lastname", TypeName = "character varying")]
    public string Lastname { get; set; } = null!;

    [Column("email", TypeName = "character varying")]
    public string Email { get; set; } = null!;
}
