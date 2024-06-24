using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using KhumaloCrafts.Models;

namespace KhumaloCrafts.Models
{
    [Table("Craft")]
    public class Craft
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? CraftName { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public double ProductPrice { get; set; }
        public string? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }
        public Stock Stock { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
        [NotMapped]
        public int Availability { get; set; }

    }
}
