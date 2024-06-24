using System.ComponentModel.DataAnnotations;

namespace KhumaloCrafts.ViewModels
{
    public class StockDTO
    {
        public int CraftId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Availability must be a non-negative value.")]
        public int Availability { get; set; }
    }
}
