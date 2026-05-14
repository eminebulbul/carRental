using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    [Required(ErrorMessage = "Şehir gereklidir")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Şehir 2 ile 100 karakter arasında olmalıdır")]
    [Display(Name = "Şehir")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Adres gereklidir")]
    [StringLength(255, MinimumLength = 5, ErrorMessage = "Adres 5 ile 255 karakter arasında olmalıdır")]
    [Display(Name = "Adres")]
    public string Address { get; set; } = null!;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Display(Name = "İade Kiralamalar")]
    public virtual ICollection<Rental> RentalDropoffBranches { get; set; } = new List<Rental>();

    [Display(Name = "Alış Kiralamalar")]
    public virtual ICollection<Rental> RentalPickupBranches { get; set; } = new List<Rental>();

    [Display(Name = "Personel")]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    [Display(Name = "Araçlar")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
