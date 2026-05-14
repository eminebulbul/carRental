using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages.Rentals
{
    public class DeleteModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IRentalService rentalService, ILogger<DeleteModel> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        public Rental Rental { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                Rental = await _rentalService.GetDetailedAsync(id.Value);
                if (Rental == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiralama yüklenirken hata: {Id}", id);
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
                var success = await _rentalService.DeleteAsync(id.Value);
                if (!success)
                {
                    return NotFound();
                }

                _logger.LogInformation("Kiralama silindi: {RentalId}", id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiralama silinirken hata");
                ModelState.AddModelError("", "Kiralama silinirken bir hata oluştu");
                return Page();
            }
        }
    }
}
