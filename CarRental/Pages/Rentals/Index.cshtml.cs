using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Rentals;

public class IndexModel : PageModel
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<Rental> Rentals { get; set; } = new List<Rental>();

    public IndexModel(IRentalService rentalService, ILogger<IndexModel> logger)
    {
        _rentalService = rentalService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Rentals = await _rentalService.GetAllDetailedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kiralamalar yüklenirken hata");
            Rentals = new List<Rental>();
        }
    }
}
