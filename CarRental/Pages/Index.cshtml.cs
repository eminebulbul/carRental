using CarRental.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarRental.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICustomerService _customerService;
    private readonly IVehicleService _vehicleService;
    private readonly IRentalService _rentalService;
    private readonly IPaymentService _paymentService;
    private readonly IDamageReportService _damageReportService;

    // Statistics Properties
    public int TotalCustomers { get; set; }
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int RentedVehicles { get; set; }
    public int MaintenanceVehicles { get; set; }
    public int ActiveRentals { get; set; }
    public int PendingRentals { get; set; }
    public int CompletedRentals { get; set; }
    public int CancelledRentals { get; set; }
    public int TotalRentals { get; set; }
    public int TotalDamageReports { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalRepairCosts { get; set; }

    public IndexModel(
        ILogger<IndexModel> logger,
        ICustomerService customerService,
        IVehicleService vehicleService,
        IRentalService rentalService,
        IPaymentService paymentService,
        IDamageReportService damageReportService)
    {
        _logger = logger;
        _customerService = customerService;
        _vehicleService = vehicleService;
        _rentalService = rentalService;
        _paymentService = paymentService;
        _damageReportService = damageReportService;
    }

    public async Task OnGetAsync()
    {
        try
        {
            // Load customer statistics
            var customers = await _customerService.GetAllAsync();
            TotalCustomers = customers.Count();

            // Load vehicle statistics
            var vehicles = await _vehicleService.GetAllAsync();
            TotalVehicles = vehicles.Count();

            var availableVehicles = await _vehicleService.GetAvailableVehiclesAsync();
            AvailableVehicles = availableVehicles.Count();

            var rentedVehicles = await _vehicleService.GetVehiclesByStatusAsync("rented");
            RentedVehicles = rentedVehicles.Count();

            var maintenanceVehicles = await _vehicleService.GetVehiclesByStatusAsync("maintenance");
            MaintenanceVehicles = maintenanceVehicles.Count();

            // Load rental statistics
            var activeRentals = await _rentalService.GetActiveRentalsAsync();
            ActiveRentals = activeRentals.Count();

            var pendingRentals = await _rentalService.GetByStatusAsync("pending");
            PendingRentals = pendingRentals.Count();

            var completedRentals = await _rentalService.GetByStatusAsync("completed");
            CompletedRentals = completedRentals.Count();

            var cancelledRentals = await _rentalService.GetByStatusAsync("cancelled");
            CancelledRentals = cancelledRentals.Count();

            var allRentals = await _rentalService.GetAllAsync();
            TotalRentals = allRentals.Count();

            // Load financial statistics (Sadece Tamamlananlar)
            TotalRevenue = allRentals.Where(r => r.Status == "completed" && r.TotalAmount.HasValue)
                                     .Sum(r => r.TotalAmount.Value);

            // Load damage report statistics
            var damageReports = await _damageReportService.GetAllAsync();
            TotalDamageReports = damageReports.Count();
            TotalRepairCosts = await _damageReportService.GetTotalRepairCostsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard statistics");
        }
    }
}
