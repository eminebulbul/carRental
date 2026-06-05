using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class DamageReport
{
    public int DamageId { get; set; }

    [Required(ErrorMessage = "Kiralama gereklidir")]
    [Display(Name = "Kiralama")]
    public int RentalId { get; set; }

    [Required(ErrorMessage = "Hasar açıklaması gereklidir")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Hasar açıklaması 10 ile 500 karakter arasında olmalıdır")]
    [Display(Name = "Hasar Açıklaması")]
    public string Description { get; set; } = null!;

    [Range(0, 1000000, ErrorMessage = "Tamir maliyeti 0 ile 1000000 arasında olmalıdır")]
    [DataType(DataType.Currency)]
    [Display(Name = "Tamir Maliyeti (₺)")]
    public decimal? RepairCost { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Rapor Tarihi")]
    public DateOnly? ReportDate { get; set; }

    [Display(Name = "Kiralama")]
    public virtual Rental? Rental { get; set; }
}
