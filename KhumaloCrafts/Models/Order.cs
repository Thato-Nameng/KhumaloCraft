using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCrafts.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public int OrderStatusId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(30)]
        [EmailAddress]
        public string Email { get; set; }

        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
