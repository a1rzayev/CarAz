using System.ComponentModel.DataAnnotations;

namespace CarAz.Core.Entities;

public class Car
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Brand { get; set; } = string.Empty;
    
    [Required]
    public string Model { get; set; } = string.Empty;
    
    public int Year { get; set; }
    
    public decimal Price { get; set; }
    
    public string? Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public string FuelType { get; set; } = string.Empty; // Gasoline, Diesel, Electric, Hybrid
    
    public string Transmission { get; set; } = string.Empty; // Manual, Automatic
    
    public int Mileage { get; set; }
    
    public string Color { get; set; } = string.Empty;
    
    public bool IsAvailable { get; set; } = true;
    
    public string Status { get; set; } = "For Sale"; // For Sale, For Rent, Sold, Rented
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign key for user who owns/listed the car
    public Guid OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;
    
    // Navigation properties
    public virtual ICollection<FavoriteCar> FavoritedBy { get; set; } = new List<FavoriteCar>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
} 