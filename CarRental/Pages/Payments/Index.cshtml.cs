using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Payments;

public class IndexModel : PageModel
{
    private readonly IPaymentService _paymentService;

    public IndexModel(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public IList<Payment> Payments { get; set; } = default!;

    public async Task OnGetAsync()
    {
        // Tüm ödemeleri getirmek için çok geniş bir tarih aralığı veriyoruz
        var payments = await _paymentService.GetPaymentsByDateRangeAsync(DateTime.MinValue, DateTime.MaxValue);
        Payments = payments.ToList();
    }
}
