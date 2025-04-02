using System.ComponentModel.DataAnnotations;

namespace shared.DTOs.Account;

public class TokenDto
{
    [Required]
    public required string Token { get; set; }
}
