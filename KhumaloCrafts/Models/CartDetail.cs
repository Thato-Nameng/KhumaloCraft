using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCrafts.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int CraftId { get; set; }
        [Required]
        public int Availability { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        public Craft Craft { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

    }
}
