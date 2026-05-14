using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    [Display(Name = "Kategori")]
    public int? CategoryId { get; set; }

    [Display(Name = "Şube")]
    public int? BranchId { get; set; }

    [Required(ErrorMessage = "Plaka numarası gereklidir")]
    [StringLength(20, ErrorMessage = "Plaka numarası 20 karakteri geçemez")]
    [Display(Name = "Plaka Numarası")]
    public string PlateNumber { get; set; } = null!;

    [Required(ErrorMessage = "Marka gereklidir")]
    [StringLength(50, ErrorMessage = "Marka 50 karakteri geçemez")]
    [Display(Name = "Marka")]
    public string Brand { get; set; } = null!;

    [Required(ErrorMessage = "Model gereklidir")]
    [StringLength(50, ErrorMessage = "Model 50 karakteri geçemez")]
    [Display(Name = "Model")]
    public string Model { get; set; } = null!;

    [Required(ErrorMessage = "Üretim yılı gereklidir")]
    [Range(1900, 2100, ErrorMessage = "Üretim yılı 1900 ile 2100 arasında olmalıdır")]
    [Display(Name = "Üretim Yılı")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Günlük fiyat gereklidir")]
    [Range(0.01, 100000, ErrorMessage = "Günlük fiyat 0 ile 100000 arasında olmalıdır")]
    [DataType(DataType.Currency)]
    [Display(Name = "Günlük Fiyat (₺)")]
    public decimal DailyPrice { get; set; }

    [StringLength(20)]
    [RegularExpression("^(available|rented|maintenance)$", ErrorMessage = "Durum 'available', 'rented' veya 'maintenance' olmalıdır")]
    [Display(Name = "Durum")]
    public string? Status { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kilometre 0 veya daha büyük olmalıdır")]
    [Display(Name = "Kilometre")]
    public int? Mileage { get; set; }

    [Display(Name = "Şube")]
    public virtual Branch? Branch { get; set; }

    [Display(Name = "Kategori")]
    public virtual VehicleCategory? Category { get; set; }

    [Display(Name = "Kiralamalar")]
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    [Display(Name = "Özellikler")]
    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
}
