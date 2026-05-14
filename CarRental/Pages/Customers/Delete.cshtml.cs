using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ICustomerService customerService, ILogger<DeleteModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                var success = await _customerService.DeleteAsync(id.Value);
                if (!success)
                {
                    return NotFound();
                }

                _logger.LogInformation("Müşteri silindi: {CustomerId}", id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri silinirken hata");
                ModelState.AddModelError("", "Müşteri silinirken bir hata oluştu");
                return Page();
            }
        }
    }
}
