using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    [Required(ErrorMessage = "Kiralama gereklidir")]
    [Display(Name = "Kiralama")]
    public int? RentalId { get; set; }

    [Required(ErrorMessage = "Tutar gereklidir")]
    [Range(0.01, 1000000, ErrorMessage = "Tutar 0 ile 1000000 arasında olmalıdır")]
    [DataType(DataType.Currency)]
    [Display(Name = "Tutar (₺)")]
    public decimal Amount { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Ödeme Tarihi")]
    public DateTime? PaymentDate { get; set; }

    [Required(ErrorMessage = "Ödeme yöntemi gereklidir")]
    [StringLength(30)]
    [RegularExpression("^(credit_card|cash)$", ErrorMessage = "Ödeme yöntemi 'credit_card' veya 'cash' olmalıdır")]
    [Display(Name = "Ödeme Yöntemi")]
    public string? Method { get; set; }

    [Display(Name = "Kiralama")]
    public virtual Rental? Rental { get; set; }
}
