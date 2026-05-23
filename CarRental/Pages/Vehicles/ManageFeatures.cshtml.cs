using CarRental.Data;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Vehicles;

public class ManageFeaturesModel : PageModel
{
    private readonly IVehicleService _vehicleService;
    private readonly IFeatureService _featureService;
    private readonly CarRentalContext _context;
    private readonly ILogger<ManageFeaturesModel> _logger;

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public int[] SelectedFeatureIds { get; set; } = Array.Empty<int>();

    public Vehicle? Vehicle { get; private set; }
    public List<FeatureOption> AvailableFeatures { get; private set; } = [];

    public ManageFeaturesModel(
        IVehicleService vehicleService,
        IFeatureService featureService,
        CarRentalContext context,
        ILogger<ManageFeaturesModel> logger)
    {
        _vehicleService = vehicleService;
        _featureService = featureService;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        Id = id.Value;
        return await LoadPageAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Id <= 0)
        {
            return NotFound();
        }

        try
        {
            var vehicle = await _vehicleService.GetDetailedAsync(Id);
            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.Features.Clear();

            if (SelectedFeatureIds?.Any() == true)
            {
                foreach (var featureId in SelectedFeatureIds)
                {
                    var feature = await _featureService.GetByIdAsync(featureId);
                    if (feature != null)
                    {
                        vehicle.Features.Add(feature);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Araç özellikleri güncellendi.";
            return RedirectToPage("/Vehicles/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Araç özellikleri güncellenirken hata oluştu. VehicleId: {VehicleId}", Id);
            ModelState.AddModelError(string.Empty, "Özellikler güncellenirken bir hata oluştu.");
            return await LoadPageAsync();
        }
    }

    private async Task<IActionResult> LoadPageAsync()
    {
        Vehicle = await _vehicleService.GetDetailedAsync(Id);
        if (Vehicle == null)
        {
            return NotFound();
        }

        var selectedIds = Vehicle.Features.Select(feature => feature.FeatureId).ToHashSet();
        var features = await _featureService.GetAllAsync();

        AvailableFeatures = (features ?? Enumerable.Empty<Feature>())
            .Select(feature => new FeatureOption
            {
                FeatureId = feature.FeatureId,
                Name = feature.Name,
                Description = feature.Description,
                IsSelected = selectedIds.Contains(feature.FeatureId)
            })
            .OrderBy(feature => feature.Name)
            .ToList();

        SelectedFeatureIds = selectedIds.ToArray();
        return Page();
    }

    public sealed class FeatureOption
    {
        public int FeatureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
    }
}
