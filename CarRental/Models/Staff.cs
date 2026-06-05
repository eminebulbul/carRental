using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    [Required(ErrorMessage = "Şube gereklidir")]
    [Display(Name = "Şube")]
    public int BranchId { get; set; }

    [Required(ErrorMessage = "Ad gereklidir")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad 2 ile 100 karakter arasında olmalıdır")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Soyad gereklidir")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Soyad 2 ile 100 karakter arasında olmalıdır")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Görev gereklidir")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Görev 2 ile 50 karakter arasında olmalıdır")]
    [Display(Name = "Görev")]
    public string Role { get; set; } = null!;

    [Display(Name = "Şube")]
    public virtual Branch? Branch { get; set; }
}
