using CarAz.Core.Entities;

namespace CarAz.Core.Interfaces;

public interface IFavoriteCarRepository
{
    Task<FavoriteCar?> GetByIdAsync(Guid id);
    Task<IEnumerable<FavoriteCar>> GetByUserIdAsync(Guid userId);
    Task<FavoriteCar?> GetByUserAndCarAsync(Guid userId, Guid carId);
    Task<FavoriteCar> CreateAsync(FavoriteCar favoriteCar);
    Task DeleteAsync(Guid id);
    Task DeleteByUserAndCarAsync(Guid userId, Guid carId);
    Task<bool> IsFavoritedAsync(Guid userId, Guid carId);
} 