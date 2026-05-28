using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.DamageReports;

public class EditModel : PageModel
{
    private readonly IDamageReportService _damageReportService;
    private readonly IRentalService _rentalService;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public DamageReport DamageReport { get; set; } = new();

    public SelectList RentalSelectList { get; set; } = new SelectList(Array.Empty<object>());

    public EditModel(IDamageReportService damageReportService, IRentalService rentalService, ILogger<EditModel> logger)
    {
        _damageReportService = damageReportService;
        _rentalService = rentalService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            var report = await _damageReportService.GetByIdAsync(id.Value);
            if (report == null) return NotFound();

            DamageReport = report;
            await PopulateSelectListsAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hasar raporu yüklenirken hata: {Id}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        if (!ModelState.IsValid) return Page();

        try
        {
            await _damageReportService.UpdateAsync(DamageReport);
            TempData["SuccessMessage"] = "Hasar raporu başarıyla güncellendi.";
            return RedirectToPage("/DamageReports/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hasar raporu güncellenirken hata");
            ModelState.AddModelError(string.Empty, "Güncelleme sırasında hata oluştu.");
            return Page();
        }
    }

    private async Task PopulateSelectListsAsync()
    {
        var rentals = (await _rentalService.GetAllAsync())?.Where(r => r != null).ToList() ?? new List<Rental>();
        RentalSelectList = new SelectList(rentals, "RentalId", "RentalId");
    }
}
