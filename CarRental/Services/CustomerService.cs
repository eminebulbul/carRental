using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    /// <summary>
    /// Service for Customer operations and business logic
    /// </summary>
    public class CustomerService : GenericService<Customer>, ICustomerService
    {
        public CustomerService(CarRentalContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.LicenseNumber == licenseNumber);
        }

        public async Task<Customer?> GetWithRentalsAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.Rentals)
                    .ThenInclude(r => r.Vehicle)
                        .ThenInclude(v => v.Category)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> LicenseNumberExistsAsync(string licenseNumber)
        {
            return await _dbSet.AnyAsync(c => c.LicenseNumber == licenseNumber);
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsAsync(int customerId)
        {
            return await _context.Rentals
                .Where(r => r.CustomerId == customerId && 
                           (r.Status == "pending" || r.Status == "active"))
                .Include(r => r.Vehicle)
                .Include(r => r.PickupBranch)
                .Include(r => r.DropoffBranch)
                .ToListAsync();
        }
    }
}
