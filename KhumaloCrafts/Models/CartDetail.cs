using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCrafts.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShoppingCartId { get; set; }

        [Required]
        public int CraftId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public ShoppingCart ShoppingCart { get; set; }
        public Craft Craft { get; set; }
    }
}
