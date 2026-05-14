using CarRental.Models;

namespace CarRental.Services
{
    /// <summary>
    /// Service interface for Customer operations and business logic
    /// </summary>
    public interface ICustomerService : IGenericService<Customer>
    {
        /// <summary>
        /// Get customer by license number
        /// </summary>
        Task<Customer?> GetByLicenseNumberAsync(string licenseNumber);

        /// <summary>
        /// Get customer with all rentals (including related entities)
        /// </summary>
        Task<Customer?> GetWithRentalsAsync(int customerId);

        /// <summary>
        /// Check if license number already exists
        /// </summary>
        Task<bool> LicenseNumberExistsAsync(string licenseNumber);

        /// <summary>
        /// Get customer's active rentals
        /// </summary>
        Task<IEnumerable<Rental>> GetActiveRentalsAsync(int customerId);
    }
}
