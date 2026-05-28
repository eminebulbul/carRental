using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.DamageReports;

public class CreateModel : PageModel
{
    private readonly IDamageReportService _damageReportService;
    private readonly IRentalService _rentalService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public DamageReport DamageReport { get; set; } = new();

    public SelectList RentalSelectList { get; set; } = new SelectList(Array.Empty<object>());

    public CreateModel(IDamageReportService damageReportService, IRentalService rentalService, ILogger<CreateModel> logger)
    {
        _damageReportService = damageReportService;
        _rentalService = rentalService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? rentalId)
    {
        await PopulateSelectListsAsync();

        if (rentalId.HasValue)
        {
            DamageReport.RentalId = rentalId.Value;
        }

        return Page();
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
            if (DamageReport.ReportDate == null)
            {
                DamageReport.ReportDate = DateOnly.FromDateTime(DateTime.Now);
            }

            await _damageReportService.CreateAsync(DamageReport);
            TempData["SuccessMessage"] = "Hasar raporu başarıyla eklendi.";
            return RedirectToPage("/DamageReports/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hasar raporu eklenirken hata oluştu");
            ModelState.AddModelError(string.Empty, "Kaydetme sırasında bir hata oluştu.");
            return Page();
        }
    }

    private async Task PopulateSelectListsAsync()
    {
        var rentals = (await _rentalService.GetAllAsync())?.Where(r => r != null).ToList() ?? new List<Rental>();
        RentalSelectList = new SelectList(rentals, "RentalId", "RentalId");
    }
}
