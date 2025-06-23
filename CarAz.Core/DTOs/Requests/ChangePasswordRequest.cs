using System.ComponentModel.DataAnnotations;

namespace CarAz.Core.DTOs.Requests;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
} 