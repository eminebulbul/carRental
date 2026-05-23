using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Vehicles;

public class DeleteModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<DeleteModel> _logger;

    [BindProperty]
    public Vehicle Vehicle { get; set; } = new();

    public DeleteModel(IVehicleService vehicleService, ILogger<DeleteModel> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            var v = await _vehicleService.GetDetailedAsync(id.Value);
            if (v == null) return NotFound();

            Vehicle = v;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araç yüklenirken hata: {Id}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var success = await _vehicleService.DeleteAsync(Vehicle.VehicleId);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Araç silinirken hata oluştu.");
                return Page();
            }

            TempData["SuccessMessage"] = "Araç silindi.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araç silinirken hata");
            ModelState.AddModelError(string.Empty, "Silme sırasında hata oluştu.");
            return Page();
        }
    }
}
