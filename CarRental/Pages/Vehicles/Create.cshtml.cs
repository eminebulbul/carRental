using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Vehicles;

public class CreateModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly IBranchService _branchService;
    private readonly IVehicleCategoryService _categoryService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public Vehicle Vehicle { get; set; } = new();

    public SelectList BranchSelectList { get; set; } = new SelectList(Array.Empty<object>());
    public SelectList CategorySelectList { get; set; } = new SelectList(Array.Empty<object>());

    public CreateModel(IVehicleService vehicleService,
                       IBranchService branchService,
                       IVehicleCategoryService categoryService,
                       ILogger<CreateModel> logger)
    {
        _vehicleService = vehicleService;
        _branchService = branchService;
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        await PopulateSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existing = await _vehicleService.GetByPlateNumberAsync(Vehicle.PlateNumber);
            if (existing != null)
            {
                ModelState.AddModelError("Vehicle.PlateNumber", "Bu plaka zaten kayıtlı.");
                return Page();
            }

            var created = await _vehicleService.CreateAsync(Vehicle);

            TempData["SuccessMessage"] = "Araç başarıyla eklendi.";
            return RedirectToPage("/Vehicles/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araç eklenirken hata oluştu");
            ModelState.AddModelError(string.Empty, "Kaydetme sırasında bir hata oluştu.");
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
