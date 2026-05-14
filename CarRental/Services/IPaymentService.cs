using CarRental.Models;

namespace CarRental.Services
{
    /// <summary>
    /// Service interface for Payment operations
    /// </summary>
    public interface IPaymentService : IGenericService<Payment>
    {
        /// <summary>
        /// Get payment with rental details
        /// </summary>
        Task<Payment?> GetDetailedAsync(int paymentId);

        /// <summary>
        /// Get payment by rental ID
        /// (Since each rental has max 1 payment - KR-04)
        /// </summary>
        Task<Payment?> GetByRentalIdAsync(int rentalId);

        /// <summary>
        /// Create payment for a rental
        /// Validates: rental exists, no existing payment for rental (KR-04)
        /// </summary>
        Task<(bool Success, string Message, Payment? Payment)> CreatePaymentAsync(Payment payment);

        /// <summary>
        /// Get all payments for a date range
        /// </summary>
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get total revenue
        /// </summary>
        Task<decimal> GetTotalRevenueAsync();

        /// <summary>
        /// Get revenue by payment method
        /// </summary>
        Task<Dictionary<string, decimal>> GetRevenueByMethodAsync();
    }
}
