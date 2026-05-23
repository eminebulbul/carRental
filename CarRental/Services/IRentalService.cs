using CarRental.Models;

namespace CarRental.Services
{
    /// <summary>
    /// Service interface for Rental operations and business logic
    /// </summary>
    public interface IRentalService : IGenericService<Rental>
    {
        /// <summary>
        /// Get rental with all related data (customer, vehicle, branches, payment, damage reports)
        /// </summary>
        Task<Rental?> GetDetailedAsync(int rentalId);

        /// <summary>
        /// Get rentals by status (pending, active, completed, cancelled)
        /// </summary>
        Task<IEnumerable<Rental>> GetByStatusAsync(string status);

        /// <summary>
        /// Get active rentals (pending or active status)
        /// </summary>
        Task<IEnumerable<Rental>> GetActiveRentalsAsync();

        /// <summary>
        /// Get rentals for a specific customer
        /// </summary>
        Task<IEnumerable<Rental>> GetCustomerRentalsAsync(int customerId);

            /// <summary>
            /// Get all rentals including related data (customer, vehicle, branches, payment, damage reports)
            /// </summary>
            Task<IEnumerable<Rental>> GetAllDetailedAsync();

        /// <summary>
        /// Check if a vehicle has overlapping active rentals
        /// (Prevents renting same vehicle at same time - KR-01)
        /// </summary>
        Task<bool> HasOverlappingRentalAsync(int vehicleId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Create a new rental with validation
        /// Validates: vehicle availability, customer exists, no overlapping rentals
        /// </summary>
        Task<(bool Success, string Message, Rental? Rental)> CreateRentalAsync(Rental rental);

        /// <summary>
        /// Activate a rental (pending → active)
        /// </summary>
        Task<(bool Success, string Message)> ActivateRentalAsync(int rentalId);

        /// <summary>
        /// Complete a rental (active → completed)
        /// Triggers: Vehicle status update + location change (via database trigger)
        /// </summary>
        Task<(bool Success, string Message)> CompleteRentalAsync(int rentalId);

        /// <summary>
        /// Cancel a rental
        /// </summary>
        Task<(bool Success, string Message)> CancelRentalAsync(int rentalId);

        /// <summary>
        /// Calculate rental cost based on daily rate and days
        /// </summary>
        Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate);
    }
}
