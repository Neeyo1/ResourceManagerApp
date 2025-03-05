namespace API.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string Email { get; set; }
    public DateTime ExpiryDate { get; set; }
}
