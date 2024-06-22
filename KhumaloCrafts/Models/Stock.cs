using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCrafts.Models
{
    [Table("Stock")]
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        public int CraftId { get; set; }
        public int Quantity { get; set; }

        public Craft Craft { get; set; }
    }
}
