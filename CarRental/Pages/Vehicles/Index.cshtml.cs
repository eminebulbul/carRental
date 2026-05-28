using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Vehicles;

public class IndexModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public IndexModel(IVehicleService vehicleService, ILogger<IndexModel> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var allVehicles = await _vehicleService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                allVehicles = allVehicles.Where(v => 
                    (v.PlateNumber ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (v.Brand ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (v.Model ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.VehicleId.ToString().Contains(SearchTerm)
                );
            }

            Vehicles = allVehicles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araçlar yüklenirken hata");
            Vehicles = new List<Vehicle>();
        }
    }
}
