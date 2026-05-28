using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Branches;

public class EditBranchModel : PageModel
{
    private readonly IBranchService _branchService;
    private readonly ILogger<EditBranchModel> _logger;

    [BindProperty]
    public Branch Branch { get; set; } = new();

    public EditBranchModel(IBranchService branchService, ILogger<EditBranchModel> logger)
    {
        _branchService = branchService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!id.HasValue) return NotFound();

        try
        {
            var branch = await _branchService.GetByIdAsync(id.Value);
            if (branch == null) return NotFound();

            Branch = branch;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şube yüklenirken hata: {Id}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _branchService.UpdateAsync(Branch);
            TempData["SuccessMessage"] = "Şube başarıyla güncellendi.";
            return RedirectToPage("/Branches/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şube güncellenirken hata");
            ModelState.AddModelError(string.Empty, "Güncelleme sırasında hata oluştu.");
            return Page();
        }
    }
}
