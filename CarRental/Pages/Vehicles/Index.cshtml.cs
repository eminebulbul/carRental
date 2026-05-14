using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Vehicles;

public class IndexModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<IndexModel> _logger;

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
            Vehicles = await _vehicleService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araçlar yüklenirken hata");
            Vehicles = new List<Vehicle>();
        }
    }
}
