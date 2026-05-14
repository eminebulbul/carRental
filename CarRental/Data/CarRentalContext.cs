using System;
using System.Collections.Generic;
using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data;

public partial class CarRentalContext : DbContext
{
    public CarRentalContext()
    {
    }

    public CarRentalContext(DbContextOptions<CarRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DamageReport> DamageReports { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleCategory> VehicleCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=car_rental;User Id=postgres;Password=postgres;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("branch_pkey");

            entity.ToTable("branch");

            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.HasIndex(e => e.Email, "customer_email_key").IsUnique();

            entity.HasIndex(e => e.LicenseNumber, "customer_license_number_key").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(50)
                .HasColumnName("license_number");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<DamageReport>(entity =>
        {
            entity.HasKey(e => e.DamageId).HasName("damage_report_pkey");

            entity.ToTable("damage_report");

            entity.Property(e => e.DamageId).HasColumnName("damage_id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.RepairCost)
                .HasPrecision(10, 2)
                .HasColumnName("repair_cost");
            entity.Property(e => e.ReportDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("report_date");

            entity.HasOne(d => d.Rental).WithMany(p => p.DamageReports)
                .HasForeignKey(d => d.RentalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_damage_rental");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("feature_pkey");

            entity.ToTable("feature");

            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.HasIndex(e => e.RentalId, "payment_rental_id_key").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Method)
                .HasMaxLength(30)
                .HasColumnName("method");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");

            entity.HasOne(d => d.Rental).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.RentalId)
                .HasConstraintName("fk_payment_rental");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.RentalId).HasName("rental_pkey");

            entity.ToTable("rental");

            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DropoffBranchId).HasColumnName("dropoff_branch_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.PickupBranchId).HasColumnName("pickup_branch_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("fk_rental_customer");

            entity.HasOne(d => d.DropoffBranch).WithMany(p => p.RentalDropoffBranches)
                .HasForeignKey(d => d.DropoffBranchId)
                .HasConstraintName("fk_rental_dropoff");

            entity.HasOne(d => d.PickupBranch).WithMany(p => p.RentalPickupBranches)
                .HasForeignKey(d => d.PickupBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rental_pickup");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("fk_rental_vehicle");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("staff_pkey");

            entity.ToTable("staff");

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");

            entity.HasOne(d => d.Branch).WithMany(p => p.Staff)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_staff_branch");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("vehicle_pkey");

            entity.ToTable("vehicle");

            entity.HasIndex(e => e.PlateNumber, "vehicle_plate_number_key").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Brand)
                .HasMaxLength(50)
                .HasColumnName("brand");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.DailyPrice)
                .HasPrecision(10, 2)
                .HasColumnName("daily_price");
            entity.Property(e => e.Mileage)
                .HasDefaultValue(0)
                .HasColumnName("mileage");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(20)
                .HasColumnName("plate_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Branch).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("fk_vehicle_branch");

            entity.HasOne(d => d.Category).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("fk_vehicle_category");

            entity.HasMany(d => d.Features).WithMany(p => p.Vehicles)
                .UsingEntity<Dictionary<string, object>>(
                    "VehicleFeature",
                    r => r.HasOne<Feature>().WithMany()
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_vf_feature"),
                    l => l.HasOne<Vehicle>().WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_vf_vehicle"),
                    j =>
                    {
                        j.HasKey("VehicleId", "FeatureId").HasName("vehicle_feature_pkey");
                        j.ToTable("vehicle_feature");
                        j.IndexerProperty<int>("VehicleId").HasColumnName("vehicle_id");
                        j.IndexerProperty<int>("FeatureId").HasColumnName("feature_id");
                    });
        });

        modelBuilder.Entity<VehicleCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("vehicle_category_pkey");

            entity.ToTable("vehicle_category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
