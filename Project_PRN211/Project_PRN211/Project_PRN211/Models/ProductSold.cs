using System.ComponentModel.DataAnnotations;

namespace Project_PRN211.Models
{
    public partial class ProductSold
    {
        [Key]
        public int Id { get; set; }
        public int? CartId { get; set; }

        public int? UId { get; set; }
        public int? ProductId { get; set; }
        public int? Receiving_Address { get; set; }
    }

}
