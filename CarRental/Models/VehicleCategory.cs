using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class VehicleCategory
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Kategori adı gereklidir")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Kategori adı 2 ile 50 karakter arasında olmalıdır")]
    [Display(Name = "Kategori Adı")]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Display(Name = "Araçlar")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
