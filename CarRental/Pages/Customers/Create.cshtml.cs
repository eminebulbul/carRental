using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ICustomerService customerService, ILogger<CreateModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if license number already exists
                var existingCustomer = await _customerService.GetByLicenseNumberAsync(Customer.LicenseNumber);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Customer.LicenseNumber", "Bu ehliyet numarası zaten kayıtlı");
                    return Page();
                }

                // Set creation date
                Customer.CreatedAt = DateTime.Now;

                // Create customer
                await _customerService.CreateAsync(Customer);
                _logger.LogInformation("Yeni müşteri oluşturuldu: {FirstName} {LastName}", Customer.FirstName, Customer.LastName);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri oluşturulurken hata");
                ModelState.AddModelError("", "Müşteri oluşturulurken bir hata oluştu");
                return Page();
            }
        }
    }
}
