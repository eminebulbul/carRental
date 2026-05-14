# RAZOR PAGES IMPLEMENTATION GUIDE

This guide provides templates and patterns for implementing the remaining Razor Pages in the Car Rental application.

---

## PROJECT STATUS

✅ **Completed:**
- Phase 1: Project Setup
- Phase 2: Database Integration (EF Core Scaffolding from PostgreSQL)
- Phase 3: Services Layer (8 service classes with business logic)
- Home Page Dashboard (/Index) - Statistics & Quick Links
- Shared Layout (_Layout.cshtml) - Navigation with Bootstrap 5

⏳ **To Complete:**
- Customer Management Pages (4 pages)
- Vehicle Management Pages (2 pages)
- Rental Management Pages (4 pages) - **CORE FEATURE**
- Payment Pages (2 pages)
- Damage Report Pages (2 pages)

---

## IMPLEMENTATION PATTERNS

### Pattern 1: List/Index Page (Read Only)

**File:** `Pages/Customers/Index.cshtml.cs`
```csharp
using CarRental.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarRental.Models;

namespace CarRental.Pages.Customers;

public class IndexModel : PageModel
{
    private readonly ICustomerService _customerService;

    public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();

    public IndexModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task OnGetAsync()
    {
        Customers = await _customerService.GetAllAsync();
    }
}
```

**File:** `Pages/Customers/Index.cshtml`
```html
@page
@model IndexModel
@{
    ViewData["Title"] = "Müşteri Listesi";
}

<div class="row mb-4">
    <div class="col-md-8">
        <h1>Müşteri Listesi</h1>
    </div>
    <div class="col-md-4 text-end">
        <a asp-page="Create" class="btn btn-primary">+ Yeni Müşteri</a>
    </div>
</div>

@if (Model.Customers.Any())
{
    <div class="table-responsive">
        <table class="table table-hover">
            <thead>
                <tr>
                    <th>Ad Soyad</th>
                    <th>Ehliyet</th>
                    <th>E-posta</th>
                    <th>Telefon</th>
                    <th>İşlemler</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var customer in Model.Customers)
                {
                    <tr>
                        <td>@customer.FirstName @customer.LastName</td>
                        <td>@customer.LicenseNumber</td>
                        <td>@customer.Email</td>
                        <td>@customer.Phone</td>
                        <td>
                            <a asp-page="Details" asp-route-id="@customer.CustomerId" class="btn btn-sm btn-info">Detay</a>
                            <a asp-page="Edit" asp-route-id="@customer.CustomerId" class="btn btn-sm btn-warning">Düzenle</a>
                            <a asp-page="Delete" asp-route-id="@customer.CustomerId" class="btn btn-sm btn-danger">Sil</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
}
else
{
    <div class="alert alert-info">
        <p>Henüz müşteri kaydı yok. <a asp-page="Create">Yeni müşteri eklemek için tıklayın</a></p>
    </div>
}
```

---

### Pattern 2: Create/Edit Page

**File:** `Pages/Customers/Create.cshtml.cs`
```csharp
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CarRental.Models;

namespace CarRental.Pages.Customers;

public class CreateModel : PageModel
{
    private readonly ICustomerService _customerService;

    [BindProperty]
    public Customer Customer { get; set; } = new();

    public CreateModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Check if license number already exists
            if (await _customerService.LicenseNumberExistsAsync(Customer.LicenseNumber))
            {
                ModelState.AddModelError("Customer.LicenseNumber", "Bu ehliyet numarası zaten kayıtlı");
                return Page();
            }

            Customer.CreatedAt = DateTime.Now;
            await _customerService.CreateAsync(Customer);
            TempData["SuccessMessage"] = "Müşteri başarıyla oluşturuldu";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
            return Page();
        }
    }
}
```

**File:** `Pages/Customers/Create.cshtml`
```html
@page
@model CreateModel
@{
    ViewData["Title"] = "Yeni Müşteri Oluştur";
}

<div class="row">
    <div class="col-md-8">
        <h1>Yeni Müşteri</h1>

        <form method="post">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label asp-for="Customer.FirstName" class="form-label"></label>
                            <input asp-for="Customer.FirstName" class="form-control" />
                            <span asp-validation-for="Customer.FirstName" class="text-danger"></span>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label asp-for="Customer.LastName" class="form-label"></label>
                            <input asp-for="Customer.LastName" class="form-control" />
                            <span asp-validation-for="Customer.LastName" class="text-danger"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label asp-for="Customer.LicenseNumber" class="form-label"></label>
                            <input asp-for="Customer.LicenseNumber" class="form-control" />
                            <span asp-validation-for="Customer.LicenseNumber" class="text-danger"></span>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label asp-for="Customer.BirthDate" class="form-label"></label>
                            <input asp-for="Customer.BirthDate" type="date" class="form-control" />
                            <span asp-validation-for="Customer.BirthDate" class="text-danger"></span>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label asp-for="Customer.Email" class="form-label"></label>
                            <input asp-for="Customer.Email" type="email" class="form-control" />
                            <span asp-validation-for="Customer.Email" class="text-danger"></span>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label asp-for="Customer.Phone" class="form-label"></label>
                            <input asp-for="Customer.Phone" class="form-control" />
                            <span asp-validation-for="Customer.Phone" class="text-danger"></span>
                        </div>
                    </div>
                </div>
                <div class="card-footer">
                    <button type="submit" class="btn btn-primary">Oluştur</button>
                    <a asp-page="Index" class="btn btn-secondary">İptal</a>
                </div>
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

---

### Pattern 3: Core Feature - Rental Management

**File:** `Pages/Rentals/Create.cshtml.cs`
```csharp
using CarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CarRental.Models;

namespace CarRental.Pages.Rentals;

public class CreateModel : PageModel
{
    private readonly IRentalService _rentalService;
    private readonly ICustomerService _customerService;
    private readonly IVehicleService _vehicleService;
    private readonly IBranchService _branchService;

    [BindProperty]
    public Rental Rental { get; set; } = new();

    public SelectList? CustomerSelect { get; set; }
    public SelectList? VehicleSelect { get; set; }
    public SelectList? BranchSelect { get; set; }

    public CreateModel(
        IRentalService rentalService,
        ICustomerService customerService,
        IVehicleService vehicleService,
        IBranchService branchService)
    {
        _rentalService = rentalService;
        _customerService = customerService;
        _vehicleService = vehicleService;
        _branchService = branchService;
    }

    public async Task OnGetAsync()
    {
        var customers = await _customerService.GetAllAsync();
        var availableVehicles = await _vehicleService.GetAvailableVehiclesAsync();
        var branches = await _branchService.GetAllAsync();

        CustomerSelect = new SelectList(customers, "CustomerId", "FirstName");
        VehicleSelect = new SelectList(availableVehicles, "VehicleId", "Model");
        BranchSelect = new SelectList(branches, "BranchId", "City");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        Rental.Status = "pending";
        Rental.StartDate = DateTime.Now;

        var (success, message, rental) = await _rentalService.CreateRentalAsync(Rental);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await OnGetAsync();
            return Page();
        }

        TempData["SuccessMessage"] = message;
        return RedirectToPage("Index");
    }
}
```

---

## PAGES TO IMPLEMENT

### Customers Module
1. **Index.cshtml** - List all customers (USE PATTERN 1)
2. **Create.cshtml** - Create new customer (USE PATTERN 2)
3. **Edit.cshtml** - Edit customer details (SIMILAR TO CREATE)
4. **Details.cshtml** - Show customer & rental history

### Vehicles Module
1. **Index.cshtml** - List vehicles with filters (status, category, branch)
2. **Details.cshtml** - Show vehicle details, features, rental history

### Rentals Module (CORE FEATURE)
1. **Index.cshtml** - List rentals (filterable by status)
2. **Create.cshtml** - Create new rental (USE PATTERN 3)
3. **Activate.cshtml** - Change pending → active
4. **Complete.cshtml** - Change active → completed

### Payments Module
1. **Index.cshtml** - List payments
2. **Create.cshtml** - Create payment for rental

### DamageReports Module
1. **Index.cshtml** - List damage reports
2. **Create.cshtml** - Create damage report (only for completed rentals)

---

## QUICK IMPLEMENTATION CHECKLIST

Follow this order for fastest implementation:

- [ ] Customers/Index
- [ ] Customers/Create (use Pattern 2)
- [ ] Customers/Edit (modify Pattern 2)
- [ ] Customers/Details
- [ ] Vehicles/Index
- [ ] Vehicles/Details
- [ ] Rentals/Index
- [ ] Rentals/Create (use Pattern 3)
- [ ] Rentals/Activate
- [ ] Rentals/Complete
- [ ] Payments/Index
- [ ] Payments/Create
- [ ] DamageReports/Index
- [ ] DamageReports/Create

---

## COMMON UI COMPONENTS

### Status Badge Helper
```html
@{
    var statusClass = Model.Rental.Status switch
    {
        "pending" => "badge-info",
        "active" => "badge-success",
        "completed" => "badge-secondary",
        "cancelled" => "badge-danger",
        _ => "badge-secondary"
    };
}
<span class="badge @statusClass">@Model.Rental.Status</span>
```

### Vehicle Status Badge
```html
@{
    var statusClass = vehicle.Status switch
    {
        "available" => "status-available",
        "rented" => "status-rented",
        "maintenance" => "status-maintenance",
        _ => "badge-secondary"
    };
}
<span class="badge badge-status @statusClass">@vehicle.Status</span>
```

### Pagination (if needed)
```csharp
public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex < TotalPages - 1;
}
```

---

## KEY BUSINESS LOGIC POINTS

1. **KR-01: Vehicle Conflict Check** - RentalService.CreateRentalAsync() validates no overlapping rentals
2. **KR-02: License Unique** - CustomerService.LicenseNumberExistsAsync() prevents duplicates
3. **KR-03: Damage Reports** - DamageReportService.CreateDamageReportAsync() only allows completed rentals
4. **KR-04: Payment 1:1** - PaymentService.CreatePaymentAsync() enforces one payment per rental
5. **KR-05: Trigger** - Database trigger automatically updates vehicle status when rental completes
6. **KR-06: Status Values** - CHECK constraint in database enforces valid statuses
7. **KR-07: Payment Method** - Payment model validates 'credit_card' or 'cash'
8. **KR-08: Null Dropoff** - Rental allows NULL dropoff_branch for active rentals

---

## VALIDATION ATTRIBUTES ALREADY ADDED

All models have validation annotations:
- `[Required]` for mandatory fields
- `[StringLength]` for text fields
- `[Range]` for numeric fields
- `[EmailAddress]` for email
- `[Phone]` for phone numbers
- `[RegularExpression]` for status/method fields
- `[Display]` for Turkish labels

Razor Pages use:
- `@Html.ValidationMessageFor()`
- `asp-validation-for` directives
- Bootstrap validation styling (in _Layout.cshtml)

---

## HELPFUL COMMANDS

```bash
# Run development server
dotnet run

# Build project
dotnet build

# Create new Razor Page (scaffold)
dotnet aspnet-codegenerator razorpage Create Customer Models.Customer -udl -outDir Pages/Customers

# Run tests (once added)
dotnet test
```

---

## NOTES FOR COMPLETION

1. **Services are injected via constructor** - See Index.cshtml.cs for example
2. **TempData for messages** - Use TempData["SuccessMessage"] and TempData["ErrorMessage"]
3. **ModelState validation** - Check `if (!ModelState.IsValid)` before database operations
4. **Async/await pattern** - All database calls are async
5. **Bootstrap classes** - Available in _Layout.cshtml: `.card`, `.btn`, `.table-hover`, etc.
6. **Status badges** - Use CSS classes: `.status-available`, `.status-rented`, `.status-active`, etc.

---

## DOCKER DEPLOYMENT NEXT

Once Razor Pages are complete, Phase 10 will create:
- `Dockerfile` - Multi-stage build for ASP.NET
- `docker-compose.yml` - PostgreSQL + ASP.NET services
- `.dockerignore` - Exclude unnecessary files
- `docker-entrypoint.sh` - Database initialization script

---

**Good luck!** The foundation is solid. Following the patterns above, you can complete the remaining pages in ~2 hours.
