using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Feature
{
    public int FeatureId { get; set; }

    [Required(ErrorMessage = "Özellik adı gereklidir")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Özellik adı 2 ile 100 karakter arasında olmalıdır")]
    [Display(Name = "Özellik Adı")]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Display(Name = "Araçlar")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
