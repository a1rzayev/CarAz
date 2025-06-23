namespace CarAz.Core.Entities;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public Guid CarId { get; set; }
    public virtual Car Car { get; set; } = null!;
    
    public string Type { get; set; } = string.Empty; // Test Drive, Rent, Buy
    
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public decimal? TotalPrice { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
} 