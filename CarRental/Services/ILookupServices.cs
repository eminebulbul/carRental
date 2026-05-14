using CarRental.Models;

namespace CarRental.Services
{
    /// <summary>
    /// Service interface for Branch operations
    /// </summary>
    public interface IBranchService : IGenericService<Branch>
    {
        /// <summary>
        /// Get branch with all related data (vehicles, staff, rentals)
        /// </summary>
        Task<Branch?> GetDetailedAsync(int branchId);

        /// <summary>
        /// Get branch by city name
        /// </summary>
        Task<Branch?> GetByCityAsync(string city);

        /// <summary>
        /// Get all vehicles in a branch
        /// </summary>
        Task<IEnumerable<Vehicle>> GetVehiclesAsync(int branchId);

        /// <summary>
        /// Get staff in a branch
        /// </summary>
        Task<IEnumerable<Staff>> GetStaffAsync(int branchId);
    }

    /// <summary>
    /// Service interface for Vehicle Category operations
    /// </summary>
    public interface IVehicleCategoryService : IGenericService<VehicleCategory>
    {
        /// <summary>
        /// Get category with all vehicles
        /// </summary>
        Task<VehicleCategory?> GetDetailedAsync(int categoryId);

        /// <summary>
        /// Get category by name
        /// </summary>
        Task<VehicleCategory?> GetByNameAsync(string name);

        /// <summary>
        /// Get vehicle count by category
        /// </summary>
        Task<Dictionary<string, int>> GetVehicleCountByCategoryAsync();
    }

    /// <summary>
    /// Service interface for Feature operations
    /// </summary>
    public interface IFeatureService : IGenericService<Feature>
    {
        /// <summary>
        /// Get feature by name
        /// </summary>
        Task<Feature?> GetByNameAsync(string name);

        /// <summary>
        /// Get all vehicles with this feature
        /// </summary>
        Task<IEnumerable<Vehicle>> GetVehiclesWithFeatureAsync(int featureId);
    }
}
