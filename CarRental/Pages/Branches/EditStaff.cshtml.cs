using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Branches;

public class EditStaffModel : PageModel
{
    private readonly IStaffService _staffService;
    private readonly IBranchService _branchService;
    private readonly ILogger<EditStaffModel> _logger;

    [BindProperty]
    public Staff Staff { get; set; } = new();

    public SelectList BranchSelectList { get; set; } = new SelectList(Array.Empty<object>());

    public EditStaffModel(IStaffService staffService, IBranchService branchService, ILogger<EditStaffModel> logger)
    {
        _staffService = staffService;
        _branchService = branchService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            var staff = await _staffService.GetByIdAsync(id.Value);
            if (staff == null) return NotFound();

            Staff = staff;
            await PopulateSelectListsAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Personel yüklenirken hata: {Id}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateSelectListsAsync();

        if (!ModelState.IsValid) return Page();

        try
        {
            await _staffService.UpdateAsync(Staff);
            TempData["SuccessMessage"] = "Personel başarıyla güncellendi.";
            return RedirectToPage("/Branches/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Personel güncellenirken hata");
            ModelState.AddModelError(string.Empty, "Güncelleme sırasında hata oluştu.");
            return Page();
        }
    }

    private async Task PopulateSelectListsAsync()
    {
        var branches = (await _branchService.GetAllAsync())?.Where(b => b != null).ToList() ?? new List<Branch>();
        BranchSelectList = new SelectList(branches, "BranchId", "City");
    }
}
