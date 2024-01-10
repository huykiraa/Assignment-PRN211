using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_PRN211.Models;

public partial class Category
{
    [Key]
  
    public int Cid { get; set; }
    

    public string Cname { get; set; } = null!;
}
