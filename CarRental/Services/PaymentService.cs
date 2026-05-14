using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    /// <summary>
    /// Service for Payment operations
    /// </summary>
    public class PaymentService : GenericService<Payment>, IPaymentService
    {
        public PaymentService(CarRentalContext context) : base(context)
        {
        }

        public async Task<Payment?> GetDetailedAsync(int paymentId)
        {
            return await _dbSet
                .Include(p => p.Rental)
                    .ThenInclude(r => r.Customer)
                .Include(p => p.Rental)
                    .ThenInclude(r => r.Vehicle)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment?> GetByRentalIdAsync(int rentalId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.RentalId == rentalId);
        }

        public async Task<(bool Success, string Message, Payment? Payment)> CreatePaymentAsync(Payment payment)
        {
            // Validate rental exists
            var rental = await _context.Rentals.FindAsync(payment.RentalId);
            if (rental == null)
                return (false, "Kiralama bulunamadı", null);

            // KR-04: Check if payment already exists for this rental
            var existingPayment = await GetByRentalIdAsync(payment.RentalId ?? 0);
            if (existingPayment != null)
                return (false, "Bu kiralama için zaten bir ödeme kaydı mevcuttur", null);

            // Validate payment method
            if (!new[] { "credit_card", "cash" }.Contains(payment.Method))
                return (false, "Geçersiz ödeme yöntemi. 'credit_card' veya 'cash' olmalıdır", null);

            // Validate amount
            if (payment.Amount <= 0)
                return (false, "Ödeme tutarı 0'dan büyük olmalıdır", null);

            // Set payment date if not provided
            if (payment.PaymentDate == null)
                payment.PaymentDate = DateTime.Now;

            try
            {
                await CreateAsync(payment);
                return (true, "Ödeme başarıyla kaydedildi", payment);
            }
            catch (Exception ex)
            {
                return (false, $"Ödeme kaydedilirken hata: {ex.Message}", null);
            }
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .Include(p => p.Rental)
                    .ThenInclude(r => r.Customer)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbSet.SumAsync(p => p.Amount);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByMethodAsync()
        {
            var results = await _dbSet
                .GroupBy(p => p.Method)
                .Select(g => new { Method = g.Key, Total = g.Sum(p => p.Amount) })
                .ToListAsync();

            return results.ToDictionary(
                r => r.Method ?? "Bilinmiyor",
                r => r.Total
            );
        }
    }
}
