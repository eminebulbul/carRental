using CarRental.Data;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Services;

public class StaffService : GenericService<Staff>, IStaffService
{
    public StaffService(CarRentalContext context) : base(context)
    {
    }
}
