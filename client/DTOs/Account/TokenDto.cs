using System.ComponentModel.DataAnnotations;

namespace client.DTOs.Account;

public class TokenDto
{
    [Required]
    public required string Token { get; set; }
}
