using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    /// <summary>
    /// Service for Damage Report operations
    /// </summary>
    public class DamageReportService : GenericService<DamageReport>, IDamageReportService
    {
        public DamageReportService(CarRentalContext context) : base(context)
        {
        }

        public async Task<DamageReport?> GetDetailedAsync(int damageId)
        {
            return await _dbSet
                .Include(d => d.Rental)
                    .ThenInclude(r => r.Customer)
                .Include(d => d.Rental)
                    .ThenInclude(r => r.Vehicle)
                        .ThenInclude(v => v.Category)
                .FirstOrDefaultAsync(d => d.DamageId == damageId);
        }

        public async Task<IEnumerable<DamageReport>> GetByRentalIdAsync(int rentalId)
        {
            return await _dbSet
                .Where(d => d.RentalId == rentalId)
                .Include(d => d.Rental)
                .OrderByDescending(d => d.ReportDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DamageReport>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _dbSet
                .Where(d => d.Rental.VehicleId == vehicleId)
                .Include(d => d.Rental)
                    .ThenInclude(r => r.Vehicle)
                .OrderByDescending(d => d.ReportDate)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, DamageReport? Report)> CreateDamageReportAsync(DamageReport report)
        {
            // Validate rental exists
            var rental = await _context.Rentals.FindAsync(report.RentalId);
            if (rental == null)
                return (false, "Kiralama bulunamadı", null);

            // KR-03: Validate rental is completed
            if (rental.Status != "completed")
                return (false, $"Hasar raporu yalnızca tamamlanmış kiralamalar için oluşturulabilir. Mevcut durum: {rental.Status}", null);

            // Validate description
            if (string.IsNullOrWhiteSpace(report.Description))
                return (false, "Hasar açıklaması gereklidir", null);

            if (report.Description.Length < 10)
                return (false, "Hasar açıklaması en az 10 karakter olmalıdır", null);

            // Validate repair cost if provided
            if (report.RepairCost.HasValue && report.RepairCost < 0)
                return (false, "Tamir maliyeti negatif olamaz", null);

            // Set report date if not provided
            if (report.ReportDate == null)
                report.ReportDate = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                await CreateAsync(report);
                return (true, "Hasar raporu başarıyla oluşturuldu", report);
            }
            catch (Exception ex)
            {
                return (false, $"Hasar raporu oluşturulurken hata: {ex.Message}", null);
            }
        }

        public async Task<decimal> GetTotalRepairCostsAsync()
        {
            return await _dbSet
                .Where(d => d.RepairCost.HasValue)
                .SumAsync(d => d.RepairCost.Value);
        }

        public async Task<IEnumerable<DamageReport>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(d => d.ReportDate >= DateOnly.FromDateTime(startDate) && 
                           d.ReportDate <= DateOnly.FromDateTime(endDate))
                .Include(d => d.Rental)
                    .ThenInclude(r => r.Vehicle)
                .OrderByDescending(d => d.ReportDate)
                .ToListAsync();
        }
    }
}
