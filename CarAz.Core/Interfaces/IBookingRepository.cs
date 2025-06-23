using CarAz.Core.Entities;

namespace CarAz.Core.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Booking>> GetByCarIdAsync(Guid carId);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking> UpdateAsync(Booking booking);
    Task DeleteAsync(Guid id);
} 