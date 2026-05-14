using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Rentals
{
    public class EditModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly ICustomerService _customerService;
        private readonly IVehicleService _vehicleService;
        private readonly IBranchService _branchService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            IRentalService rentalService,
            ICustomerService customerService,
            IVehicleService vehicleService,
            IBranchService branchService,
            ILogger<EditModel> logger)
        {
            _rentalService = rentalService;
            _customerService = customerService;
            _vehicleService = vehicleService;
            _branchService = branchService;
            _logger = logger;
        }

        [BindProperty]
        public Rental Rental { get; set; } = new();

        public SelectList? Customers { get; set; }
        public SelectList? Vehicles { get; set; }
        public SelectList? Branches { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                Rental = await _rentalService.GetByIdAsync(id.Value);
                if (Rental == null)
                {
                    return NotFound();
                }

                await LoadDropdownsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiralama yüklenirken hata: {Id}", id);
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            try
            {
                await _rentalService.UpdateAsync(Rental);
                _logger.LogInformation("Kiralama güncellendi: {RentalId}", Rental.RentalId);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiralama güncellenirken hata");
                ModelState.AddModelError("", "Kiralama güncellenirken bir hata oluştu");
                await LoadDropdownsAsync();
                return Page();
            }
        }

        private async Task LoadDropdownsAsync()
        {
            try
            {
                var customers = await _customerService.GetAllAsync();
                Customers = new SelectList(customers, "CustomerId", "FirstName");

                var vehicles = await _vehicleService.GetAllAsync();
                Vehicles = new SelectList(vehicles, "VehicleId", "Model");

                var branches = await _branchService.GetAllAsync();
                Branches = new SelectList(branches, "BranchId", "City");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown'lar yüklenirken hata");
            }
        }
    }
}
