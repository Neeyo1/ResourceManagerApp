namespace API.DTOs.Account;

public class UserDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Token { get; set; }
}
