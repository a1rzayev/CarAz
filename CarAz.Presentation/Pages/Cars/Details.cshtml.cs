using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarAz.Core.Interfaces;
using CarAz.Core.Entities;

namespace CarAz.Presentation.Pages.Cars;

public class DetailsModel : PageModel
{
    private readonly ICarRepository _carRepository;

    public DetailsModel(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public Car Car { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Car = await _carRepository.GetByIdAsync(id);
        
        if (Car == null)
        {
            return NotFound();
        }

        return Page();
    }
} 