using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Branches;

public class CreateStaffModel : PageModel
{
    private readonly IStaffService _staffService;
    private readonly IBranchService _branchService;
    private readonly ILogger<CreateStaffModel> _logger;

    [BindProperty]
    public Staff Staff { get; set; } = new();

    public SelectList BranchSelectList { get; set; } = new SelectList(Array.Empty<object>());

    public CreateStaffModel(IStaffService staffService, IBranchService branchService, ILogger<CreateStaffModel> logger)
    {
        _staffService = staffService;
        _branchService = branchService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateSelectListsAsync();
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
            await _staffService.CreateAsync(Staff);
            TempData["SuccessMessage"] = "Personel başarıyla eklendi.";
            return RedirectToPage("/Branches/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Personel eklenirken hata oluştu");
            ModelState.AddModelError(string.Empty, "Kaydetme sırasında bir hata oluştu.");
            return Page();
        }
    }

    private async Task PopulateSelectListsAsync()
    {
        var branches = (await _branchService.GetAllAsync())?.Where(b => b != null).ToList() ?? new List<Branch>();
        BranchSelectList = new SelectList(branches, "BranchId", "City");
    }
}
