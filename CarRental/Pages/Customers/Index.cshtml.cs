using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<IndexModel> _logger;

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
            Customers = await _customerService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Müşteriler yüklenirken hata");
            Customers = new List<Customer>();
        }
    }
}
