using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Branches;

public class CreateBranchModel : PageModel
{
    private readonly IBranchService _branchService;
    private readonly ILogger<CreateBranchModel> _logger;

    [BindProperty]
    public Branch Branch { get; set; } = new();

    public CreateBranchModel(IBranchService branchService, ILogger<CreateBranchModel> logger)
    {
        _branchService = branchService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _branchService.CreateAsync(Branch);
            TempData["SuccessMessage"] = "Şube başarıyla eklendi.";
            return RedirectToPage("/Branches/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şube eklenirken hata oluştu");
            ModelState.AddModelError(string.Empty, "Kaydetme sırasında bir hata oluştu.");
            return Page();
        }
    }
}
