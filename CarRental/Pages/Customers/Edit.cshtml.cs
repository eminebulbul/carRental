using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ICustomerService customerService, ILogger<EditModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                Customer = await _customerService.GetByIdAsync(id.Value);
                if (Customer == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri yüklenirken hata: {Id}", id);
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _customerService.UpdateAsync(Customer);
                _logger.LogInformation("Müşteri güncellendi: {CustomerId}", Customer.CustomerId);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri güncellenirken hata");
                ModelState.AddModelError("", "Müşteri güncellenirken bir hata oluştu");
                return Page();
            }
        }
    }
}
