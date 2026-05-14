using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRental.Pages.Rentals
{
    public class CreateModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly ICustomerService _customerService;
        private readonly IVehicleService _vehicleService;
        private readonly IBranchService _branchService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IRentalService rentalService,
            ICustomerService customerService,
            IVehicleService vehicleService,
            IBranchService branchService,
            ILogger<CreateModel> logger)
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

        public async Task OnGetAsync()
        {
            await LoadDropdownsAsync();
            Rental.Status = "pending";
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
                // Use business logic validation
                var result = await _rentalService.CreateRentalAsync(Rental);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    await LoadDropdownsAsync();
                    return Page();
                }

                _logger.LogInformation("Yeni kiralama oluşturuldu: {RentalId}", result.Rental?.RentalId);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiralama oluşturulurken hata");
                ModelState.AddModelError("", "Kiralama oluşturulurken bir hata oluştu");
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
