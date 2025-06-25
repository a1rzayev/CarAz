using Microsoft.EntityFrameworkCore;
using CarAz.Core.Entities;
using CarAz.Core.Interfaces;
using CarAz.Infrastructure.Data;

namespace CarAz.Infrastructure.Repositories;

public class FavoriteCarRepository : IFavoriteCarRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteCarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FavoriteCar?> GetByIdAsync(Guid id)
    {
        return await _context.FavoriteCars
            .Include(f => f.User)
            .Include(f => f.Car)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<FavoriteCar?> GetByUserAndCarAsync(Guid userId, Guid carId)
    {
        return await _context.FavoriteCars
            .Include(f => f.User)
            .Include(f => f.Car)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
    }

    public async Task<IEnumerable<FavoriteCar>> GetByUserIdAsync(Guid userId)
    {
        return await _context.FavoriteCars
            .Include(f => f.User)
            .Include(f => f.Car)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<FavoriteCar>> GetByCarAsync(Guid carId)
    {
        return await _context.FavoriteCars
            .Include(f => f.User)
            .Include(f => f.Car)
            .Where(f => f.CarId == carId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<FavoriteCar> CreateAsync(FavoriteCar favoriteCar)
    {
        _context.FavoriteCars.Add(favoriteCar);
        await _context.SaveChangesAsync();
        return favoriteCar;
    }

    public async Task DeleteAsync(Guid id)
    {
        var favoriteCar = await _context.FavoriteCars.FindAsync(id);
        if (favoriteCar != null)
        {
            _context.FavoriteCars.Remove(favoriteCar);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByUserAndCarAsync(Guid userId, Guid carId)
    {
        var favoriteCar = await _context.FavoriteCars
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
        
        if (favoriteCar != null)
        {
            _context.FavoriteCars.Remove(favoriteCar);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsFavoritedAsync(Guid userId, Guid carId)
    {
        return await _context.FavoriteCars
            .AnyAsync(f => f.UserId == userId && f.CarId == carId);
    }

    public async Task<int> GetFavoriteCountAsync(Guid carId)
    {
        return await _context.FavoriteCars
            .CountAsync(f => f.CarId == carId);
    }
} 