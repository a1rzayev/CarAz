using System.ComponentModel.DataAnnotations;

namespace CarAz.Core.DTOs.Responses;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Message { get; set; }
    public UserDto? User { get; set; }
}