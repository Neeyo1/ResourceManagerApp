using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account;

public class UserEditPasswordDto
{
    [Required]
    [StringLength(20, MinimumLength = 8)]
    public required string CurrentPassword { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 8)]
    public required string NewPassword { get; set; }
}
