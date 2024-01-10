using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_PRN211.Models;

public partial class Cart
{
    
    public int CartId { get; set; }
    public int? AccountId { get; set; }

    public int? ProductId { get; set; }

    public int? Amount { get; set; }
    public int? Receiving_AddressId { get; set; }
}
