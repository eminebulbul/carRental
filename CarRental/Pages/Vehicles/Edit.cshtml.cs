using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Vehicles;

public class EditModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly IBranchService _branchService;
    private readonly IVehicleCategoryService _categoryService;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public Vehicle Vehicle { get; set; } = new();

    public SelectList BranchSelectList { get; set; } = null!;
    public SelectList CategorySelectList { get; set; } = null!;

    public EditModel(IVehicleService vehicleService,
                     IBranchService branchService,
                     IVehicleCategoryService categoryService,
                     ILogger<EditModel> logger)
    {
        _vehicleService = vehicleService;
        _branchService = branchService;
        _categoryService = categoryService;
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
            await PopulateSelectListsAsync();
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
        await PopulateSelectListsAsync();

        if (!ModelState.IsValid) return Page();

        try
        {
            var existing = await _vehicleService.GetByIdAsync(Vehicle.VehicleId);
            if (existing == null) return NotFound();

            // Update scalar properties
            existing.PlateNumber = Vehicle.PlateNumber;
            existing.Brand = Vehicle.Brand;
            existing.Model = Vehicle.Model;
            existing.Year = Vehicle.Year;
            existing.DailyPrice = Vehicle.DailyPrice;
            existing.Mileage = Vehicle.Mileage;
            existing.Status = Vehicle.Status;
            existing.CategoryId = Vehicle.CategoryId;
            existing.BranchId = Vehicle.BranchId;

            await _vehicleService.UpdateAsync(existing);
            TempData["SuccessMessage"] = "Araç güncellendi.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araç güncellenirken hata");
            ModelState.AddModelError(string.Empty, "Güncelleme sırasında hata oluştu.");
            return Page();
        }
    }

    private async Task PopulateSelectListsAsync()
    {
        var branches = (await _branchService.GetAllAsync())?.Where(b => b != null).ToList() ?? new List<Models.Branch>();
        var categories = (await _categoryService.GetAllAsync())?.Where(c => c != null).ToList() ?? new List<Models.VehicleCategory>();

        BranchSelectList = new SelectList(branches, "BranchId", "Name");
        CategorySelectList = new SelectList(categories, "CategoryId", "Name");
    }
}
