using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account;

public class TokenDto
{
    [Required]
    public required string Token { get; set; }
}
