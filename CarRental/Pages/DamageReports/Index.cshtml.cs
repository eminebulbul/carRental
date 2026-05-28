using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.DamageReports;

public class IndexModel : PageModel
{
    private readonly IDamageReportService _damageReportService;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public List<DamageReport> DamageReports { get; set; } = new();
    public int? FilterRentalId { get; set; }

    public IndexModel(IDamageReportService damageReportService, ILogger<IndexModel> logger)
    {
        _damageReportService = damageReportService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? rentalId)
    {
        try
        {
            var reports = await _damageReportService.GetAllAsync();
            DamageReports = reports?.ToList() ?? new List<DamageReport>();

            if (rentalId.HasValue)
            {
                FilterRentalId = rentalId;
                DamageReports = DamageReports.Where(d => d.RentalId == rentalId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                DamageReports = DamageReports.Where(d => 
                    d.DamageId.ToString().Contains(SearchTerm) ||
                    d.RentalId.ToString().Contains(SearchTerm) ||
                    (d.Description ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hasar raporları yüklenirken hata oluştu");
            TempData["ErrorMessage"] = "Veriler yüklenirken bir hata oluştu.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            await _damageReportService.DeleteAsync(id.Value);
            TempData["SuccessMessage"] = "Hasar raporu başarıyla silindi.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hasar raporu silinirken hata: {Id}", id);
            TempData["ErrorMessage"] = "Hasar raporu silinirken bir hata oluştu.";
            return RedirectToPage();
        }
    }
}
