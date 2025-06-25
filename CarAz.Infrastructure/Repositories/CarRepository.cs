using Microsoft.EntityFrameworkCore;
using CarAz.Core.Entities;
using CarAz.Core.Interfaces;
using CarAz.Infrastructure.Data;

namespace CarAz.Infrastructure.Repositories;

public class CarRepository : ICarRepository
{
    private readonly ApplicationDbContext _context;

    public CarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Car?> GetByIdAsync(Guid id)
    {
        return await _context.Cars
            .Include(c => c.Owner)
            .Include(c => c.FavoritedBy)
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Car>> GetAllAsync()
    {
        return await _context.Cars
            .Include(c => c.Owner)
            .Include(c => c.FavoritedBy)
            .Include(c => c.Bookings)
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Cars
            .Include(c => c.Owner)
            .Include(c => c.FavoritedBy)
            .Include(c => c.Bookings)
            .Where(c => c.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
    {
        return await _context.Cars
            .Include(c => c.Owner)
            .Include(c => c.FavoritedBy)
            .Include(c => c.Bookings)
            .Where(c => c.IsAvailable && c.Status == "For Sale")
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> SearchAsync(string? brand = null, string? model = null, 
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = _context.Cars
            .Include(c => c.Owner)
            .Include(c => c.FavoritedBy)
            .Include(c => c.Bookings)
            .Where(c => c.IsAvailable);

        if (!string.IsNullOrEmpty(brand))
            query = query.Where(c => c.Brand.Contains(brand));

        if (!string.IsNullOrEmpty(model))
            query = query.Where(c => c.Model.Contains(model));

        if (minPrice.HasValue)
            query = query.Where(c => c.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.Price <= maxPrice.Value);

        return await query.ToListAsync();
    }

    public async Task<Car> CreateAsync(Car car)
    {
        _context.Cars.Add(car);
        await _context.SaveChangesAsync();
        return car;
    }

    public async Task<Car> UpdateAsync(Car car)
    {
        _context.Cars.Update(car);
        await _context.SaveChangesAsync();
        return car;
    }

    public async Task DeleteAsync(Guid id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }
} 