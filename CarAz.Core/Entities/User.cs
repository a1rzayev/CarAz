using System.ComponentModel.DataAnnotations;

namespace CarAz.Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    
    public string Role { get; set; } = "User"; // User, Admin, Dealer
    
    public bool IsEmailVerified { get; set; } = false;
    
    public string? EmailVerificationToken { get; set; }
    
    public DateTime? EmailVerificationTokenExpiry { get; set; }
    
    public string? PasswordResetToken { get; set; }
    
    public DateTime? PasswordResetTokenExpiry { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Car> OwnedCars { get; set; } = new List<Car>();
    public virtual ICollection<FavoriteCar> FavoriteCars { get; set; } = new List<FavoriteCar>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
} 