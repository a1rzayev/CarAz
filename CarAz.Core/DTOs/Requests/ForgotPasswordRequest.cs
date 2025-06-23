using System.ComponentModel.DataAnnotations;

namespace CarAz.Core.DTOs.Requests;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}