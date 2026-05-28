using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Rentals;

public class IndexModel : PageModel
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

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
            var allRentals = await _rentalService.GetAllDetailedAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                allRentals = allRentals.Where(r => 
                    r.RentalId.ToString().Contains(SearchTerm) ||
                    (r.Customer?.FirstName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (r.Customer?.LastName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (r.Customer?.Email ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (r.Vehicle?.PlateNumber ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (r.Vehicle?.Brand ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            Rentals = allRentals;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kiralamalar yüklenirken hata");
            Rentals = new List<Rental>();
        }
    }
}
