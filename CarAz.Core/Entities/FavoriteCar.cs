namespace CarAz.Core.Entities;

public class FavoriteCar
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public Guid CarId { get; set; }
    public virtual Car Car { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 