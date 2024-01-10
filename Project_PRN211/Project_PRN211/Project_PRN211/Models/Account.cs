using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_PRN211.Models;

public partial class Account
{
    public int UId { get; set; }

    public string? User { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
       ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string? Pass { get; set; }

    public int? IsSell { get; set; }

    public int? IsAdmin { get; set; }

    public string? Gmail { get; set; }
    public string? ResetPasswordCode { get; set; }
}
