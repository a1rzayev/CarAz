using Microsoft.AspNetCore.Mvc.RazorPages;
using CarAz.Core.Interfaces;
using CarAz.Core.Entities;

namespace CarAz.Presentation.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public IndexModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IEnumerable<User> Users { get; set; } = new List<User>();

    public async Task OnGetAsync()
    {
        Users = await _userRepository.GetAllAsync();
    }
} 