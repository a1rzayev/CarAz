using Microsoft.EntityFrameworkCore;
using CarAz.Core.Entities;
using CarAz.Core.Interfaces;
using CarAz.Infrastructure.Data;

namespace CarAz.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByCarIdAsync(Guid carId)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .Where(b => b.CarId == carId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByStatusAsync(string status)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Car)
            .Where(b => b.StartDate > now && b.Status == "Confirmed")
            .OrderBy(b => b.StartDate)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking> UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasConflictingBookingAsync(Guid carId, DateTime startDate, DateTime? endDate)
    {
        var query = _context.Bookings
            .Where(b => b.CarId == carId && 
                       b.Status == "Confirmed" &&
                       b.StartDate <= endDate);

        if (endDate.HasValue)
        {
            query = query.Where(b => b.EndDate >= startDate);
        }

        return await query.AnyAsync();
    }
} 