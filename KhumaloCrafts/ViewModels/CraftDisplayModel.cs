using KhumaloCrafts.Models;
using KhumaloCrafts.ViewModels;

namespace KhumaloCrafts.ViewModels
{
    public class CraftDisplayModel
    {
        public IEnumerable<Craft> Crafts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string STerm { get; set; } = "";
        public int CategoryId { get; set; } = 0;
    }
}
