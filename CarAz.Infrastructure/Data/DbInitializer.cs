using CarAz.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAz.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Users.AnyAsync())
        {
            return; // Database already seeded
        }

        // Seed sample users
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@caraz.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Role = "Admin",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "dealer@caraz.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Dealer123!"),
                FirstName = "John",
                LastName = "Dealer",
                PhoneNumber = "+1234567891",
                DateOfBirth = new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                Role = "Dealer",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "user@caraz.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                FirstName = "Jane",
                LastName = "Customer",
                PhoneNumber = "+1234567892",
                DateOfBirth = new DateTime(1995, 8, 20, 0, 0, 0, DateTimeKind.Utc),
                Role = "User",
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Get user IDs for relationships
        var adminUser = await context.Users.FirstAsync(u => u.Email == "admin@caraz.com");
        var dealerUser = await context.Users.FirstAsync(u => u.Email == "dealer@caraz.com");
        var customerUser = await context.Users.FirstAsync(u => u.Email == "user@caraz.com");

        // Seed sample cars
        var cars = new List<Car>
        {
            new Car
            {
                Id = Guid.NewGuid(),
                Brand = "Toyota",
                Model = "Camry",
                Year = 2022,
                Price = 25000.00m,
                Description = "Excellent condition, low mileage, one owner",
                ImageUrl = "https://example.com/camry.jpg",
                FuelType = "Gasoline",
                Transmission = "Automatic",
                Mileage = 15000,
                Color = "Silver",
                IsAvailable = true,
                Status = "For Sale",
                OwnerId = dealerUser.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Car
            {
                Id = Guid.NewGuid(),
                Brand = "Honda",
                Model = "Civic",
                Year = 2021,
                Price = 22000.00m,
                Description = "Well maintained, great fuel economy",
                ImageUrl = "https://example.com/civic.jpg",
                FuelType = "Gasoline",
                Transmission = "Automatic",
                Mileage = 25000,
                Color = "Blue",
                IsAvailable = true,
                Status = "For Sale",
                OwnerId = dealerUser.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Car
            {
                Id = Guid.NewGuid(),
                Brand = "Tesla",
                Model = "Model 3",
                Year = 2023,
                Price = 45000.00m,
                Description = "Electric vehicle, autopilot enabled",
                ImageUrl = "https://example.com/tesla.jpg",
                FuelType = "Electric",
                Transmission = "Automatic",
                Mileage = 8000,
                Color = "White",
                IsAvailable = true,
                Status = "For Sale",
                OwnerId = adminUser.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Car
            {
                Id = Guid.NewGuid(),
                Brand = "BMW",
                Model = "X5",
                Year = 2020,
                Price = 55000.00m,
                Description = "Luxury SUV, premium features",
                ImageUrl = "https://example.com/bmw.jpg",
                FuelType = "Gasoline",
                Transmission = "Automatic",
                Mileage = 35000,
                Color = "Black",
                IsAvailable = true,
                Status = "For Sale",
                OwnerId = dealerUser.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();

        // Get car IDs for relationships
        var camry = await context.Cars.FirstAsync(c => c.Model == "Camry");
        var civic = await context.Cars.FirstAsync(c => c.Model == "Civic");
        var tesla = await context.Cars.FirstAsync(c => c.Model == "Model 3");

        // Seed sample bookings
        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = customerUser.Id,
                CarId = camry.Id,
                Type = "Test Drive",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(1).AddHours(2),
                Status = "Confirmed",
                Notes = "Customer interested in test driving the Camry",
                CreatedAt = DateTime.UtcNow
            },
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = customerUser.Id,
                CarId = tesla.Id,
                Type = "Test Drive",
                StartDate = DateTime.UtcNow.AddDays(3),
                EndDate = DateTime.UtcNow.AddDays(3).AddHours(1),
                Status = "Pending",
                Notes = "Interested in electric vehicle",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();

        // Seed sample favorites
        var favorites = new List<FavoriteCar>
        {
            new FavoriteCar
            {
                Id = Guid.NewGuid(),
                UserId = customerUser.Id,
                CarId = camry.Id,
                CreatedAt = DateTime.UtcNow
            },
            new FavoriteCar
            {
                Id = Guid.NewGuid(),
                UserId = customerUser.Id,
                CarId = tesla.Id,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.FavoriteCars.AddRangeAsync(favorites);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Sample data seeded successfully!");
    }
} 