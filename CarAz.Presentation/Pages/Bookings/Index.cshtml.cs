using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CarAz.Core.Interfaces;
using CarAz.Core.Entities;

namespace CarAz.Presentation.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly IBookingRepository _bookingRepository;

    public IndexModel(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
    
    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? TypeFilter { get; set; }

    public SelectList StatusOptions { get; set; } = null!;

    public async Task OnGetAsync()
    {
        // Set up status options
        StatusOptions = new SelectList(new[]
        {
            "Pending",
            "Confirmed", 
            "Completed",
            "Cancelled"
        });

        if (!string.IsNullOrEmpty(StatusFilter))
        {
            Bookings = await _bookingRepository.GetByStatusAsync(StatusFilter);
        }
        else
        {
            Bookings = await _bookingRepository.GetAllAsync();
        }

        // Apply type filter if specified
        if (!string.IsNullOrEmpty(TypeFilter))
        {
            Bookings = Bookings.Where(b => b.Type == TypeFilter);
        }
    }
} 