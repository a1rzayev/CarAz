using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarAz.Core.Interfaces;
using CarAz.Core.Entities;

namespace CarAz.Presentation.Pages.Cars;

public class IndexModel : PageModel
{
    private readonly ICarRepository _carRepository;

    public IndexModel(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public IEnumerable<Car> Cars { get; set; } = new List<Car>();
    
    [BindProperty(SupportsGet = true)]
    public CarSearchModel SearchModel { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (string.IsNullOrEmpty(SearchModel.Brand) && 
            string.IsNullOrEmpty(SearchModel.Model) && 
            !SearchModel.MinPrice.HasValue && 
            !SearchModel.MaxPrice.HasValue)
        {
            Cars = await _carRepository.GetAvailableCarsAsync();
        }
        else
        {
            Cars = await _carRepository.SearchAsync(
                SearchModel.Brand, 
                SearchModel.Model, 
                SearchModel.MinPrice, 
                SearchModel.MaxPrice);
        }
    }
}

public class CarSearchModel
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
} 