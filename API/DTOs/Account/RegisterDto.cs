using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account;

public class RegisterDto
{
    [Required]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 8)]
    public required string Password { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 2)]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public required string LastName { get; set; }
}
