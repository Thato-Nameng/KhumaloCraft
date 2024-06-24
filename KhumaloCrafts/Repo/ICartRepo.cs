using KhumaloCrafts.Models;
using KhumaloCrafts.ViewModels;
using System.Threading.Tasks;

namespace KhumaloCrafts.Repo
{
    public interface ICartRepo
    {
        Task<int> AddItem(int craftId, int avail);
        Task<int> RemoveItem(int craftId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        Task<ShoppingCart> GetCart(string userId);
        Task<bool> DoCheckout(CheckoutModel model);
    }
}