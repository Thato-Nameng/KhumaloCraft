using KhumaloCrafts.Models;

namespace KhumaloCrafts.ViewModels
{
    public class OrderDetailModelDTO
    {
        public string DivId { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
