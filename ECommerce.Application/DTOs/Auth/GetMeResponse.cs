namespace ECommerce.Application.DTOs.Auth;

public class GetMeResponse
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}