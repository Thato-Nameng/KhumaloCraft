using KhumaloCrafts.Models;
using KhumaloCrafts.ViewModels;

namespace KhumaloCrafts.Repo
{
    public interface IUserOrderRepo
    {
        Task<IEnumerable<Order>> UserOrders(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusModel data);
        Task TogglePaymentStatus(int orderId);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses();
    }
}