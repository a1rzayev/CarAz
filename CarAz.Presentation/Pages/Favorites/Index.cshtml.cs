using Microsoft.AspNetCore.Mvc.RazorPages;
using CarAz.Core.Interfaces;
using CarAz.Core.Entities;

namespace CarAz.Presentation.Pages.Favorites;

public class IndexModel : PageModel
{
    private readonly IFavoriteCarRepository _favoriteCarRepository;
    private readonly IUserRepository _userRepository;

    public IndexModel(IFavoriteCarRepository favoriteCarRepository, IUserRepository userRepository)
    {
        _favoriteCarRepository = favoriteCarRepository;
        _userRepository = userRepository;
    }

    public IEnumerable<FavoriteCar> FavoriteCars { get; set; } = new List<FavoriteCar>();

    public async Task OnGetAsync()
    {
        // TODO: Get current user ID from authentication
        // For now, we'll use the first user in the system
        var users = await _userRepository.GetAllAsync();
        var currentUser = users.FirstOrDefault();
        
        if (currentUser != null)
        {
            FavoriteCars = await _favoriteCarRepository.GetByUserIdAsync(currentUser.Id);
        }
    }
} 