using System.ComponentModel.DataAnnotations;

namespace CarAz.Core.DTOs.Requests;

public class RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
} 