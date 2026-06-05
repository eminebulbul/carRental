using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Payments;

public class CreateModel : PageModel
{
    private readonly IPaymentService _paymentService;
    private readonly IRentalService _rentalService;

    public CreateModel(IPaymentService paymentService, IRentalService rentalService)
    {
        _paymentService = paymentService;
        _rentalService = rentalService;
    }

    [BindProperty]
    public Payment Payment { get; set; } = default!;

    public SelectList Rentals { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? rentalId)
    {
        await LoadSelectListsAsync();
        
        Payment = new Payment
        {
            PaymentDate = DateTime.Now,
            RentalId = rentalId
        };
        
        if (rentalId.HasValue)
        {
            var rental = await _rentalService.GetByIdAsync(rentalId.Value);
            if (rental != null && rental.TotalAmount.HasValue)
            {
                Payment.Amount = rental.TotalAmount.Value;
            }
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        var (success, message, _) = await _paymentService.CreatePaymentAsync(Payment);
        
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            await LoadSelectListsAsync();
            return Page();
        }

        TempData["SuccessMessage"] = message;
        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var rentals = await _rentalService.GetAllDetailedAsync();
        var validRentals = rentals.Where(r => r.Status != "cancelled").ToList();
        
        var rentalItems = validRentals.Select(r => new 
        {
            r.RentalId,
            DisplayText = $"#{r.RentalId} - {r.Customer?.FirstName} {r.Customer?.LastName} ({r.Vehicle?.PlateNumber}) - ₺{(r.TotalAmount ?? 0):N2}"
        });
        
        Rentals = new SelectList(rentalItems, "RentalId", "DisplayText");
    }
}
