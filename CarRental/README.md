# Car Rental System - ASP.NET Core Razor Pages

A university coursework project (BLM2058 Database Management) demonstrating a car rental system database design and web application.

## Project Structure

```
CarRental/
├── Program.cs                 # Application entry point and DI configuration
├── appsettings.json          # Main configuration (DB connection string)
├── appsettings.Development.json
├── database_schema.sql       # Full PostgreSQL schema with tables and triggers
├── sample_data.sql           # Test data for demonstration
├── Models/                   # EF Core entity models (auto-generated)
├── Data/
│   └── CarRentalContext.cs  # EF Core DbContext
├── Services/                 # Business logic layer
├── Pages/
│   ├── Shared/              # Layout and shared components
│   ├── Customers/           # Customer CRUD pages
│   ├── Vehicles/            # Vehicle management pages
│   ├── Rentals/             # Rental (core feature) pages
│   ├── Payments/            # Payment pages
│   └── DamageReports/       # Damage report pages
└── wwwroot/                 # Static files (CSS, JS, images)
```

## Database Entities

The system manages **10 core entities**:

1. **CUSTOMER** - Client information (license number, email, phone)
2. **VEHICLE_CATEGORY** - Vehicle types (economy, SUV, luxury, etc.)
3. **BRANCH** - Company branch locations (Ankara, Istanbul, Izmir, etc.)
4. **VEHICLE** - Individual vehicles with status tracking
5. **FEATURE** - Vehicle features (GPS, A/C, Camera, etc.)
6. **VEHICLE_FEATURE** - Many-to-many linking table
7. **RENTAL** - Rental transactions with status (pending→active→completed)
8. **PAYMENT** - Payment records (1:1 with RENTAL)
9. **DAMAGE_REPORT** - Damage reports for completed rentals
10. **STAFF** - Branch employees

## Prerequisites

- **.NET SDK 9.0+** (or 8.0 LTS)
- **PostgreSQL 12+**
- **Visual Studio Code** or Visual Studio 2022

## Setup Instructions

### Step 1: Install PostgreSQL

**macOS (using Homebrew):**
```bash
brew install postgresql@15
brew services start postgresql@15
```

**Windows:**
Download and install from https://www.postgresql.org/download/windows/

**Linux (Ubuntu/Debian):**
```bash
sudo apt-get install postgresql postgresql-contrib
sudo systemctl start postgresql
```

### Step 2: Create Database and Tables

```bash
# Connect to PostgreSQL as default user
psql -U postgres

# Create database
CREATE DATABASE car_rental;

# Exit psql
\q

# Load the schema
psql -U postgres -d car_rental -f database_schema.sql

# Load sample data (optional, for testing)
psql -U postgres -d car_rental -f sample_data.sql
```

**Verify database setup:**
```bash
psql -U postgres -d car_rental -c "SELECT COUNT(*) as table_count FROM information_schema.tables WHERE table_schema='public';"
# Should show: table_count: 10
```

### Step 3: Configure Connection String

Edit `appsettings.json` and update the PostgreSQL connection string if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=car_rental;User Id=postgres;Password=postgres;"
  }
}
```

**Note:** Update `User Id` and `Password` to match your PostgreSQL setup.

### Step 4: Generate EF Core Models (Database-First Scaffolding)

Run this command from the `CarRental/` directory to generate entity models from the existing database:

```bash
cd CarRental

# Full scaffolding (generates DbContext and all models)
dotnet ef dbcontext scaffold "Server=localhost;Port=5432;Database=car_rental;User Id=postgres;Password=postgres;" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models --context CarRentalContext --context-dir Data --force

# If you need to regenerate, use --force flag
```

**What this does:**
- Scans your PostgreSQL database
- Generates C# model classes for all 10 tables
- Creates/updates the `CarRentalContext.cs` DbContext
- Automatically configures foreign keys and relationships

### Step 5: Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Application will be available at https://localhost:5001
```

## Key Features Implemented

### ✅ Customer Management
- Create, read, update, delete customers
- Track license number (unique identifier)
- View customer rental history

### ✅ Vehicle Management
- Browse vehicle catalog by category and branch
- Track vehicle status (available / rented / maintenance)
- View vehicle features and rental history

### ✅ Rental System (Core)
- Create rental with validation (vehicle must be available)
- Status transitions: pending → active → completed → closed
- Support for different pickup and dropoff branches
- Auto-calculation of rental duration and cost

### ✅ Payment Processing
- Record payments linked to rentals (1:1 relationship enforced)
- Support for credit card and cash payments
- Payment history and records

### ✅ Damage Reporting
- Create damage reports only for completed rentals
- Track repair costs
- Maintain damage history

### ✅ Business Logic & Constraints
- **KR-01:** Prevent overlapping active rentals for same vehicle
- **KR-02:** Enforce unique license number per customer
- **KR-03:** Allow damage reports only on completed rentals
- **KR-04:** Limit to 1 payment per rental (UNIQUE constraint)
- **KR-05:** Auto-update vehicle status when rental completes (Database Trigger)
- **KR-06:** Vehicle status limited to 3 values (CHECK constraint)
- **KR-07:** Payment methods restricted to credit_card or cash
- **KR-08:** Allow NULL dropoff_branch for active rentals

## Database Trigger

The system includes a PostgreSQL trigger that automatically updates vehicle status and location when a rental is completed:

**Trigger:** `trg_rental_completed`

When rental status changes to 'completed':
- Vehicle status automatically changes to 'available'
- Vehicle's current branch location updates to the dropoff branch
- NULL safety: if no dropoff branch specified, vehicle stays at current branch

## Architecture

### Data Flow
```
Pages (Razor Pages) 
    ↓
PageModels (handler logic)
    ↓
Services (business logic & validation)
    ↓
EF Core DbContext
    ↓
PostgreSQL Database
```

### Technology Stack
- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 10.0+
- **Database:** PostgreSQL 12+
- **UI:** Razor Pages + Bootstrap 5
- **Language:** C# 12

## Development Notes

### Running Migrations (Future Schema Changes)

If you modify the database schema, regenerate models:

```bash
dotnet ef dbcontext scaffold "Server=localhost;Port=5432;Database=car_rental;User Id=postgres;Password=postgres;" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models --force
```

### Debugging EF Core Queries

Enable SQL query logging in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

## Common Issues

### Issue: "Connection refused" error
**Solution:** Ensure PostgreSQL is running
```bash
# Check PostgreSQL service status
brew services list  # macOS
sudo systemctl status postgresql  # Linux
```

### Issue: "Database 'car_rental' does not exist"
**Solution:** Run database creation step again or load schema:
```bash
psql -U postgres -c "CREATE DATABASE car_rental;"
psql -U postgres -d car_rental -f database_schema.sql
```

### Issue: "Npgsql version incompatibility"
**Solution:** The project uses compatible versions. If upgrading .NET:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.4
```

## Testing

### Sample Data Scenarios

The `sample_data.sql` file includes:
- 3 branches (Ankara, Istanbul, Izmir)
- 13 vehicles across categories
- 5 test customers
- 5 sample rentals (pending, active, completed, cancelled)
- 2 payments and damage reports for verification

### Quick Test Queries

```bash
# Check total entities in database
psql -U postgres -d car_rental -c "SELECT 'CUSTOMER' as table_name, COUNT(*) FROM CUSTOMER UNION ALL SELECT 'VEHICLE', COUNT(*) FROM VEHICLE UNION ALL SELECT 'RENTAL', COUNT(*) FROM RENTAL;"

# View active rentals
psql -U postgres -d car_rental -c "SELECT r.rental_id, c.first_name, v.brand, v.model, r.status FROM RENTAL r JOIN CUSTOMER c ON r.customer_id = c.customer_id JOIN VEHICLE v ON r.vehicle_id = v.vehicle_id WHERE r.status = 'active';"

# Check trigger functionality
psql -U postgres -d car_rental -c "SELECT rental_id, status, (SELECT status FROM VEHICLE WHERE vehicle_id = RENTAL.vehicle_id) as vehicle_status FROM RENTAL WHERE status = 'completed';"
```

## Docker Deployment (Phase 10)

Coming soon: Docker + Docker Compose setup for containerized deployment.

## Project Status

| Phase | Status | Description |
|-------|--------|-------------|
| 1 | ✅ Complete | Project setup, folder structure, dependencies |
| 2 | 🔄 In Progress | EF Core scaffolding from PostgreSQL |
| 3 | ⏳ Pending | Service layer implementation |
| 4-7 | ⏳ Pending | Razor Pages (CRUD modules) |
| 8 | ⏳ Pending | UI layout and Bootstrap styling |
| 9 | ⏳ Pending | Validation and business logic |
| 10 | ⏳ Pending | Docker containerization |
| 11 | ⏳ Pending | Testing and finalization |

## Credits

**Course:** BLM2058 Veritabanı Yönetimi (Database Management)  
**Institution:** Ankara University, Faculty of Engineering  
**Department:** Computer Engineering  
**Student:** Emine Bülbül (21290189)  
**Date:** April-May 2026

## License

Academic use only - University project coursework.
