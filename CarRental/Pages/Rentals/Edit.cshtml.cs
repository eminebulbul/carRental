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
        private readonly IPaymentService _paymentService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            IRentalService rentalService,
            ICustomerService customerService,
            IVehicleService vehicleService,
            IBranchService branchService,
            IPaymentService paymentService,
            ILogger<EditModel> logger)
        {
            _rentalService = rentalService;
            _customerService = customerService;
            _vehicleService = vehicleService;
            _branchService = branchService;
            _paymentService = paymentService;
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
                // Tutarı yeniden hesapla (tarih değişmiş olabilir)
                if (Rental.EndDate.HasValue && Rental.VehicleId.HasValue)
                {
                    Rental.TotalAmount = await _rentalService.CalculateRentalCostAsync(
                        Rental.VehicleId.Value, Rental.StartDate, Rental.EndDate.Value);
                }

                await _rentalService.UpdateAsync(Rental);
                _logger.LogInformation("Kiralama güncellendi: {RentalId}", Rental.RentalId);

                // Eğer durum "active" (Kirada) yapıldıysa aracı "rented" olarak işaretle
                if (Rental.Status == "active" && Rental.VehicleId.HasValue)
                {
                    await _vehicleService.UpdateStatusAsync(Rental.VehicleId.Value, "rented");
                }

                // Eğer kiralama iptal edildiyse aracı müsait (available) yap
                if (Rental.Status == "cancelled" && Rental.VehicleId.HasValue)
                {
                    await _vehicleService.UpdateStatusAsync(Rental.VehicleId.Value, "available");
                }

                // Form üzerinden manuel "completed" (tamamlandı) yapıldıysa otomatik ödemeyi yakala
                if (Rental.Status == "completed" && Rental.TotalAmount.HasValue && Rental.TotalAmount.Value > 0)
                {
                    var existingPayment = await _paymentService.GetByRentalIdAsync(Rental.RentalId);
                    if (existingPayment == null)
                    {
                        var autoPayment = new Payment
                        {
                            RentalId = Rental.RentalId,
                            Amount = Rental.TotalAmount.Value,
                            Method = "credit_card",
                            PaymentDate = DateTime.Now
                        };
                        await _paymentService.CreatePaymentAsync(autoPayment);
                    }
                }

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
                Customers = new SelectList(
                    customers.Select(c => new { c.CustomerId, Display = $"{c.FirstName} {c.LastName}" }),
                    "CustomerId", "Display");

                var vehicles = await _vehicleService.GetAllAsync();
                Vehicles = new SelectList(
                    vehicles.Select(v => new { v.VehicleId, Display = $"{v.Brand} {v.Model} ({v.PlateNumber}) - ₺{v.DailyPrice}/gün" }),
                    "VehicleId", "Display");

                var branches = await _branchService.GetAllAsync();
                Branches = new SelectList(branches, "BranchId", "City");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown'lar yüklenirken hata");
            }
        }

        // Razor view'dan çağrılıyor — JavaScript'e araç fiyat ve şube bilgilerini aktarmak için
        public async Task<IEnumerable<Vehicle>> GetVehiclesWithPricesAsync()
        {
            var available = await _vehicleService.GetAvailableVehiclesAsync();
            var rented = await _vehicleService.GetVehiclesByStatusAsync("rented");
            var maintenance = await _vehicleService.GetVehiclesByStatusAsync("maintenance");
            return available.Concat(rented).Concat(maintenance);
        }
    }
}
