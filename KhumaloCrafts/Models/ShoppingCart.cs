using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCrafts.Models
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<CartDetail> CartDetails { get; set; }
    }

}
