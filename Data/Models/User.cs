using System.ComponentModel.DataAnnotations;

namespace LegelisteApp.Data.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Username { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    public UserRole Role { get; set; }
}
