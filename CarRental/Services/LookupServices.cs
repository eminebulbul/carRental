using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    /// <summary>
    /// Service for Branch operations
    /// </summary>
    public class BranchService : GenericService<Branch>, IBranchService
    {
        public BranchService(CarRentalContext context) : base(context)
        {
        }

        public async Task<Branch?> GetDetailedAsync(int branchId)
        {
            return await _dbSet
                .Include(b => b.Vehicles)
                .Include(b => b.Staff)
                .Include(b => b.RentalPickupBranches)
                .FirstOrDefaultAsync(b => b.BranchId == branchId);
        }

        public async Task<Branch?> GetByCityAsync(string city)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.City == city);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(int branchId)
        {
            var branch = await _dbSet
                .Include(b => b.Vehicles)
                    .ThenInclude(v => v.Category)
                .FirstOrDefaultAsync(b => b.BranchId == branchId);

            return branch?.Vehicles ?? new List<Vehicle>();
        }

        public async Task<IEnumerable<Staff>> GetStaffAsync(int branchId)
        {
            var branch = await _dbSet
                .Include(b => b.Staff)
                .FirstOrDefaultAsync(b => b.BranchId == branchId);

            return branch?.Staff ?? new List<Staff>();
        }
    }

    /// <summary>
    /// Service for Vehicle Category operations
    /// </summary>
    public class VehicleCategoryService : GenericService<VehicleCategory>, IVehicleCategoryService
    {
        public VehicleCategoryService(CarRentalContext context) : base(context)
        {
        }

        public async Task<VehicleCategory?> GetDetailedAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<VehicleCategory?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Dictionary<string, int>> GetVehicleCountByCategoryAsync()
        {
            var results = await _dbSet
                .Select(c => new { c.Name, Count = c.Vehicles.Count })
                .ToListAsync();

            return results.ToDictionary(r => r.Name, r => r.Count);
        }
    }

    /// <summary>
    /// Service for Feature operations
    /// </summary>
    public class FeatureService : GenericService<Feature>, IFeatureService
    {
        public FeatureService(CarRentalContext context) : base(context)
        {
        }

        public async Task<Feature?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.Name == name);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesWithFeatureAsync(int featureId)
        {
            var feature = await _dbSet
                .Include(f => f.Vehicles)
                .FirstOrDefaultAsync(f => f.FeatureId == featureId);

            return feature?.Vehicles ?? new List<Vehicle>();
        }
    }
}
