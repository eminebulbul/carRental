using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Adı gereklidir")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad 2 ile 100 karakter arasında olmalıdır")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Soyadı gereklidir")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Soyad 2 ile 100 karakter arasında olmalıdır")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Ehliyet numarası gereklidir")]
    [StringLength(50, ErrorMessage = "Ehliyet numarası 50 karakteri geçemez")]
    [Display(Name = "Ehliyet Numarası")]
    public string LicenseNumber { get; set; } = null!;

    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi")]
    public DateOnly? BirthDate { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
    [StringLength(150)]
    [Display(Name = "E-posta")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Display(Name = "Kayıt Tarihi")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Kiralamalar")]
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
