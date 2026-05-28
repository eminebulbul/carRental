using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Branches;

public class IndexModel : PageModel
{
    private readonly IBranchService _branchService;
    private readonly IStaffService _staffService;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public List<Branch> Branches { get; set; } = new();
    public List<Staff> Staff { get; set; } = new();

    public IndexModel(IBranchService branchService, IStaffService staffService, ILogger<IndexModel> logger)
    {
        _branchService = branchService;
        _staffService = staffService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var branches = await _branchService.GetAllAsync();
            var staff = await _staffService.GetAllAsync();

            Branches = branches?.ToList() ?? new List<Branch>();
            Staff = staff?.ToList() ?? new List<Staff>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Branches = Branches.Where(b => 
                    (b.City ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (b.Address ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (b.Phone ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                Staff = Staff.Where(s => 
                    (s.FirstName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.LastName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.Role ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şubeler ve personel yüklenirken hata oluştu");
            TempData["ErrorMessage"] = "Veriler yüklenirken bir hata oluştu.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteBranchAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            await _branchService.DeleteAsync(id.Value);
            TempData["SuccessMessage"] = "Şube başarıyla silindi.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şube silinirken hata: {Id}", id);
            TempData["ErrorMessage"] = "Şube silinirken bir hata oluştu.";
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            await _staffService.DeleteAsync(id.Value);
            TempData["SuccessMessage"] = "Personel başarıyla silindi.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Personel silinirken hata: {Id}", id);
            TempData["ErrorMessage"] = "Personel silinirken bir hata oluştu.";
            return RedirectToPage();
        }
    }
}
