using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    /// <summary>
    /// Service for Rental operations and business logic
    /// Core service for managing rental lifecycle (pending → active → completed)
    /// </summary>
    public class RentalService : GenericService<Rental>, IRentalService
    {
        private readonly IVehicleService _vehicleService;

        public RentalService(CarRentalContext context, IVehicleService vehicleService) : base(context)
        {
            _vehicleService = vehicleService;
        }

        public async Task<Rental?> GetDetailedAsync(int rentalId)
        {
            return await _dbSet
                .Include(r => r.Customer)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(r => r.PickupBranch)
                .Include(r => r.DropoffBranch)
                .Include(r => r.Payment)
                .Include(r => r.DamageReports)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId);
        }

        public async Task<IEnumerable<Rental>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(r => r.Status == status)
                .Include(r => r.Customer)
                .Include(r => r.Vehicle)
                .Include(r => r.PickupBranch)
                .Include(r => r.DropoffBranch)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
        {
            return await _dbSet
                .Where(r => r.Status == "pending" || r.Status == "active")
                .Include(r => r.Customer)
                .Include(r => r.Vehicle)
                .Include(r => r.PickupBranch)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetCustomerRentalsAsync(int customerId)
        {
            return await _dbSet
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(r => r.PickupBranch)
                .Include(r => r.DropoffBranch)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingRentalAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            // KR-01: Check if vehicle has overlapping active rentals
            return await _dbSet.AnyAsync(r =>
                r.VehicleId == vehicleId &&
                (r.Status == "active" || r.Status == "pending") &&
                r.StartDate < endDate &&
                (r.EndDate == null || r.EndDate > startDate)
            );
        }

        public async Task<(bool Success, string Message, Rental? Rental)> CreateRentalAsync(Rental rental)
        {
            // Validate customer exists
            if (rental.CustomerId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(rental.CustomerId);
                if (customer == null)
                    return (false, "Müşteri bulunamadı", null);
            }

            // Validate vehicle exists
            var vehicle = await _vehicleService.GetDetailedAsync(rental.VehicleId ?? 0);
            if (vehicle == null)
                return (false, "Araç bulunamadı", null);

            // KR-01: Bakımdaki araçlar hiçbir şekilde kiralanamaz
            if (vehicle.Status == "maintenance")
                return (false, "Bu araç şu anda bakımda olduğu için kiralanamaz", null);

            // KR-01: Tarih çakışması kontrolü — aynı araç aynı tarihlerde iki kez kiralanamaz
            // (pending veya active durumundaki mevcut kiralamalarla çakışma kontrolü)
            if (await HasOverlappingRentalAsync(rental.VehicleId ?? 0, rental.StartDate, rental.EndDate ?? rental.StartDate))
                return (false, "Bu araç seçilen tarih aralığında zaten kiralanmış veya rezerve edilmiş", null);

            // Validate date range
            if (rental.EndDate.HasValue && rental.EndDate <= rental.StartDate)
                return (false, "Bitiş tarihi başlangıç tarihinden sonra olmalıdır", null);

            // Set initial status
            if (string.IsNullOrEmpty(rental.Status))
                rental.Status = "pending";

            // Calculate total amount if end date provided
            if (rental.EndDate.HasValue && rental.TotalAmount == null)
            {
                rental.TotalAmount = await CalculateRentalCostAsync(rental.VehicleId ?? 0, rental.StartDate, rental.EndDate.Value);
            }

            try
            {
                await CreateAsync(rental);
                
                // Eğer baştan "active" olarak oluşturulduysa aracı "rented" (Kirada) olarak işaretle
                if (rental.Status == "active" && rental.VehicleId.HasValue)
                {
                    await _vehicleService.UpdateStatusAsync(rental.VehicleId.Value, "rented");
                }

                return (true, "Kiralama başarıyla oluşturuldu", rental);
            }
            catch (Exception ex)
            {
                return (false, $"Kiralama oluşturulurken hata: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> ActivateRentalAsync(int rentalId)
        {
            var rental = await GetDetailedAsync(rentalId);
            if (rental == null)
                return (false, "Kiralama bulunamadı");

            if (rental.Status != "pending")
                return (false, $"Yalnızca pending durumundaki kiralamalar aktif edilebilir. Mevcut durum: {rental.Status}");

            // Aracın güncel şubesini al — araç bu sürede başka şubeye taşınmış olabilir
            var vehicle = await _vehicleService.GetByIdAsync(rental.VehicleId ?? 0);
            if (vehicle == null)
                return (false, "Araç bulunamadı");

            // Alış şubesini aracın ŞU ANKİ gerçek konumuna güncelle
            if (vehicle.BranchId.HasValue)
            {
                rental.PickupBranchId = vehicle.BranchId.Value;
            }

            rental.Status = "active";

            // Update vehicle status to 'rented'
            if (!await _vehicleService.UpdateStatusAsync(rental.VehicleId ?? 0, "rented"))
                return (false, "Araç durumu güncellenirken hata oluştu");

            try
            {
                await UpdateAsync(rental);
                return (true, "Kiralama başarıyla aktif hale getirildi. Alış şubesi aracın güncel konumuna güncellendi.");
            }
            catch (Exception ex)
            {
                return (false, $"Kiralama aktif edilirken hata: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CompleteRentalAsync(int rentalId)
        {
            var rental = await GetDetailedAsync(rentalId);
            if (rental == null)
                return (false, "Kiralama bulunamadı");

            if (rental.Status != "active")
                return (false, $"Yalnızca active durumundaki kiralamalar tamamlanabilir. Mevcut durum: {rental.Status}");

            rental.Status = "completed";
            rental.EndDate = DateTime.Now;

            // Calculate final cost if not already set
            if (rental.TotalAmount == null || rental.TotalAmount == 0)
            {
                rental.TotalAmount = await CalculateRentalCostAsync(
                    rental.VehicleId ?? 0,
                    rental.StartDate,
                    rental.EndDate.Value
                );
            }

            try
            {
                await UpdateAsync(rental);
                
                // Otomatik ödeme kaydı oluştur (Eğer daha önceden ödeme alınmadıysa)
                var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.RentalId == rental.RentalId);
                if (existingPayment == null && rental.TotalAmount.HasValue && rental.TotalAmount.Value > 0)
                {
                    var autoPayment = new Payment
                    {
                        RentalId = rental.RentalId,
                        Amount = rental.TotalAmount.Value,
                        Method = "credit_card", // Varsayılan ödeme yöntemi
                        PaymentDate = DateTime.Now
                    };
                    _context.Payments.Add(autoPayment);
                    await _context.SaveChangesAsync();
                }

                // Note: Database trigger (trg_rental_completed) will automatically update vehicle status to 'available'
                // and update its branch_id to dropoff_branch_id
                return (true, "Kiralama başarıyla tamamlandı. Araç müsait hale getirildi ve ödeme kaydı otomatik oluşturuldu.");
            }
            catch (Exception ex)
            {
                return (false, $"Kiralama tamamlanırken hata: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CancelRentalAsync(int rentalId)
        {
            var rental = await GetDetailedAsync(rentalId);
            if (rental == null)
                return (false, "Kiralama bulunamadı");

            if (rental.Status == "completed" || rental.Status == "cancelled")
                return (false, $"Bu durumda kiralama iptal edilemez. Mevcut durum: {rental.Status}");

            var wasActive = rental.Status == "active";
            rental.Status = "cancelled";

            // If rental was active, update vehicle status back to available
            if (wasActive)
            {
                if (!await _vehicleService.UpdateStatusAsync(rental.VehicleId ?? 0, "available"))
                    return (false, "Araç durumu güncellenirken hata oluştu");
            }

            try
            {
                await UpdateAsync(rental);
                return (true, "Kiralama başarıyla iptal edildi");
            }
            catch (Exception ex)
            {
                return (false, $"Kiralama iptal edilirken hata: {ex.Message}");
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var rental = await GetByIdAsync(id);
            if (rental == null)
                return false;

            // If the rental was active, update vehicle status back to available
            if (rental.Status == "active")
            {
                await _vehicleService.UpdateStatusAsync(rental.VehicleId ?? 0, "available");
            }

            return await base.DeleteAsync(id);
        }

        public async Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            var vehicle = await _vehicleService.GetByIdAsync(vehicleId);
            if (vehicle == null)
                return 0;

            int days = (int)(endDate - startDate).TotalDays;
            if (days <= 0)
                days = 1;

            return vehicle.DailyPrice * days;
        }

        public async Task<IEnumerable<Rental>> GetAllDetailedAsync()
        {
            return await _dbSet
                .Include(r => r.Customer)
                .Include(r => r.Vehicle)
                    .ThenInclude(v => v.Category)
                .Include(r => r.PickupBranch)
                .Include(r => r.DropoffBranch)
                .Include(r => r.Payment)
                .Include(r => r.DamageReports)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }
    }
}
