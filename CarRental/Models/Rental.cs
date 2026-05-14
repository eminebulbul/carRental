using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    [Display(Name = "Müşteri")]
    public int? CustomerId { get; set; }

    [Display(Name = "Araç")]
    public int? VehicleId { get; set; }

    [Required(ErrorMessage = "Alış şubesi gereklidir")]
    [Display(Name = "Alış Şubesi")]
    public int PickupBranchId { get; set; }

    [Display(Name = "İade Şubesi")]
    public int? DropoffBranchId { get; set; }

    [Required(ErrorMessage = "Başlangıç tarihi gereklidir")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Başlangıç Tarihi")]
    public DateTime StartDate { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Bitiş Tarihi")]
    public DateTime? EndDate { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Toplam Tutar (₺)")]
    public decimal? TotalAmount { get; set; }

    [StringLength(20)]
    [RegularExpression("^(pending|active|completed|cancelled)$", ErrorMessage = "Durum 'pending', 'active', 'completed' veya 'cancelled' olmalıdır")]
    [Display(Name = "Durum")]
    public string? Status { get; set; }

    [Display(Name = "Müşteri")]
    public virtual Customer? Customer { get; set; }

    [Display(Name = "Hasar Raporları")]
    public virtual ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();

    [Display(Name = "İade Şubesi")]
    public virtual Branch? DropoffBranch { get; set; }

    [Display(Name = "Ödeme")]
    public virtual Payment? Payment { get; set; }

    [Display(Name = "Alış Şubesi")]
    public virtual Branch PickupBranch { get; set; } = null!;

    [Display(Name = "Araç")]
    public virtual Vehicle? Vehicle { get; set; }
}
