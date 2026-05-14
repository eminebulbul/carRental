using CarRental.Models;

namespace CarRental.Services
{
    /// <summary>
    /// Service interface for Damage Report operations
    /// </summary>
    public interface IDamageReportService : IGenericService<DamageReport>
    {
        /// <summary>
        /// Get damage report with rental details
        /// </summary>
        Task<DamageReport?> GetDetailedAsync(int damageId);

        /// <summary>
        /// Get damage reports for a specific rental
        /// </summary>
        Task<IEnumerable<DamageReport>> GetByRentalIdAsync(int rentalId);

        /// <summary>
        /// Get damage reports for a specific vehicle
        /// </summary>
        Task<IEnumerable<DamageReport>> GetByVehicleIdAsync(int vehicleId);

        /// <summary>
        /// Create damage report for a rental
        /// Validates: rental exists and has completed status (KR-03)
        /// </summary>
        Task<(bool Success, string Message, DamageReport? Report)> CreateDamageReportAsync(DamageReport report);

        /// <summary>
        /// Get total repair costs
        /// </summary>
        Task<decimal> GetTotalRepairCostsAsync();

        /// <summary>
        /// Get damage reports for a date range
        /// </summary>
        Task<IEnumerable<DamageReport>> GetReportsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
