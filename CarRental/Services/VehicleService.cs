using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services
{
    /// <summary>
    /// Service for Vehicle operations and business logic
    /// </summary>
    public class VehicleService : GenericService<Vehicle>, IVehicleService
    {
        public VehicleService(CarRentalContext context) : base(context)
        {
        }

        public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
        }

        public async Task<Vehicle?> GetDetailedAsync(int vehicleId)
        {
            return await _dbSet
                .Include(v => v.Features)
                .Include(v => v.Category)
                .Include(v => v.Branch)
                .Include(v => v.Rentals)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _dbSet
                .Where(v => v.Status == "available")
                .Include(v => v.Category)
                .Include(v => v.Branch)
                .Include(v => v.Features)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesByBranchAsync(int branchId)
        {
            return await _dbSet
                .Where(v => v.BranchId == branchId && v.Status == "available")
                .Include(v => v.Category)
                .Include(v => v.Features)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(v => v.CategoryId == categoryId)
                .Include(v => v.Branch)
                .Include(v => v.Features)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(v => v.Status == status)
                .Include(v => v.Category)
                .Include(v => v.Branch)
                .ToListAsync();
        }

        public async Task<bool> IsAvailableAsync(int vehicleId)
        {
            var vehicle = await _dbSet.FindAsync(vehicleId);
            return vehicle?.Status == "available";
        }

        public async Task<bool> UpdateStatusAsync(int vehicleId, string newStatus)
        {
            var vehicle = await _dbSet.FindAsync(vehicleId);
            if (vehicle == null)
                return false;

            if (!new[] { "available", "rented", "maintenance" }.Contains(newStatus))
                return false;

            vehicle.Status = newStatus;
            _dbSet.Update(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Rental>> GetRentalHistoryAsync(int vehicleId)
        {
            return await _context.Rentals
                .Where(r => r.VehicleId == vehicleId)
                .Include(r => r.Customer)
                .Include(r => r.PickupBranch)
                .Include(r => r.DropoffBranch)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feature>> GetFeaturesAsync(int vehicleId)
        {
            var vehicle = await _dbSet
                .Include(v => v.Features)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            return vehicle?.Features ?? new List<Feature>();
        }
    }
}
