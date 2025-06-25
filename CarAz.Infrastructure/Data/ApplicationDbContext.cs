using Microsoft.EntityFrameworkCore;
using CarAz.Core.Entities;

namespace CarAz.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<FavoriteCar> FavoriteCars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(500);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(500);
        });

        // Car configuration
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.FuelType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Transmission).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            // Foreign key relationship
            entity.HasOne(e => e.Owner)
                  .WithMany(e => e.OwnedCars)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Foreign key relationships
            entity.HasOne(e => e.User)
                  .WithMany(e => e.Bookings)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Car)
                  .WithMany(e => e.Bookings)
                  .HasForeignKey(e => e.CarId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FavoriteCar configuration
        modelBuilder.Entity<FavoriteCar>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Foreign key relationships
            entity.HasOne(e => e.User)
                  .WithMany(e => e.FavoriteCars)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Car)
                  .WithMany(e => e.FavoritedBy)
                  .HasForeignKey(e => e.CarId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Composite unique constraint to prevent duplicate favorites
            entity.HasIndex(e => new { e.UserId, e.CarId }).IsUnique();
        });
    }
} 