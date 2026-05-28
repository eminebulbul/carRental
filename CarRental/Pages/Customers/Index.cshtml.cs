using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();

    public IndexModel(ICustomerService customerService, ILogger<IndexModel> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var allCustomers = await _customerService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                allCustomers = allCustomers.Where(c => 
                    (c.FirstName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.LastName ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Phone ?? "").Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            Customers = allCustomers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Müşteriler yüklenirken hata");
            Customers = new List<Customer>();
        }
    }
}
