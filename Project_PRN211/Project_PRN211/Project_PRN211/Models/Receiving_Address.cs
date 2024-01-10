using System.ComponentModel.DataAnnotations;

namespace Project_PRN211.Models
{
    public class Receiving_Address
    {

        [Key]
        public int AddressId { get; set; }
        public string? Address { get; set; }
        public int ? UId { get; set; }
    }
}
