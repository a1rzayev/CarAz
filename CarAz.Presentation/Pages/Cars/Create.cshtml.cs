using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarAz.Core.Interfaces;
using CarAz.Core.Entities;

namespace CarAz.Presentation.Pages.Cars;

public class CreateModel : PageModel
{
    private readonly ICarRepository _carRepository;
    private readonly IUserRepository _userRepository;

    public CreateModel(ICarRepository carRepository, IUserRepository userRepository)
    {
        _carRepository = carRepository;
        _userRepository = userRepository;
    }

    [BindProperty]
    public Car Car { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: Get current user ID from authentication
        // For now, we'll use a default user (first user in the system)
        var users = await _userRepository.GetAllAsync();
        var defaultUser = users.FirstOrDefault();
        
        if (defaultUser == null)
        {
            ModelState.AddModelError("", "No users found in the system. Please create a user first.");
            return Page();
        }

        Car.OwnerId = defaultUser.Id;
        Car.CreatedAt = DateTime.UtcNow;

        await _carRepository.CreateAsync(Car);

        return RedirectToPage("./Index");
    }
} 