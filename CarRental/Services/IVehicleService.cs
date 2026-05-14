using CarRental.Models;

namespace CarRental.Services
{
    /// <summary>
    /// Service interface for Vehicle operations and business logic
    /// </summary>
    public interface IVehicleService : IGenericService<Vehicle>
    {
        /// <summary>
        /// Get vehicle by plate number
        /// </summary>
        Task<Vehicle?> GetByPlateNumberAsync(string plateNumber);

        /// <summary>
        /// Get vehicle with all related data (features, rentals, category, branch)
        /// </summary>
        Task<Vehicle?> GetDetailedAsync(int vehicleId);

        /// <summary>
        /// Get all available vehicles (status = 'available')
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();

        /// <summary>
        /// Get available vehicles in a specific branch
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesByBranchAsync(int branchId);

        /// <summary>
        /// Get available vehicles in a specific category
        /// </summary>
        Task<IEnumerable<Vehicle>> GetVehiclesByCategoryAsync(int categoryId);

        /// <summary>
        /// Get vehicles by status
        /// </summary>
        Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(string status);

        /// <summary>
        /// Check if vehicle is available for rental
        /// </summary>
        Task<bool> IsAvailableAsync(int vehicleId);

        /// <summary>
        /// Update vehicle status
        /// </summary>
        Task<bool> UpdateStatusAsync(int vehicleId, string newStatus);

        /// <summary>
        /// Get vehicle rental history
        /// </summary>
        Task<IEnumerable<Rental>> GetRentalHistoryAsync(int vehicleId);

        /// <summary>
        /// Get vehicle features
        /// </summary>
        Task<IEnumerable<Feature>> GetFeaturesAsync(int vehicleId);
    }
}
