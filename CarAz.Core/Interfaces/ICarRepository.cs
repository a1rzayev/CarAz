using CarAz.Core.Entities;

namespace CarAz.Core.Interfaces;

public interface ICarRepository
{
    Task<Car?> GetByIdAsync(Guid id);
    Task<IEnumerable<Car>> GetAllAsync();
    Task<IEnumerable<Car>> GetByOwnerIdAsync(Guid ownerId);
    Task<IEnumerable<Car>> GetAvailableCarsAsync();
    Task<Car> CreateAsync(Car car);
    Task<Car> UpdateAsync(Car car);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Car>> SearchAsync(string? brand = null, string? model = null, decimal? minPrice = null, decimal? maxPrice = null);
} 