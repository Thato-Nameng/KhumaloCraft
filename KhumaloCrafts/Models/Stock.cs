using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCrafts.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public int CraftId { get; set; }
        public int Availability { get; set; }

        public Craft? Craft { get; set; }
    }
}