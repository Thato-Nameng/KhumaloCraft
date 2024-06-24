using KhumaloCrafts.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using KhumaloCrafts.Repo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Mono.TextTemplating;
using KhumaloCrafts.ViewModels;


namespace KhumaloCrafts.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminOperationsController : Controller
    {
        private readonly IUserOrderRepo _userOrderRepo;
        public AdminOperationsController(IUserOrderRepo userOrderRepo)
        {
            _userOrderRepo = userOrderRepo;
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderRepo.UserOrders(true);
            return View(orders);
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderRepo.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction(nameof(AllOrders));
        }

        public async Task<IActionResult> UpdatePaymentStatus(int orderId)
        {
            var order = await _userOrderRepo.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with id:{orderId} does not found.");
            }
            var orderStatusList = (await _userOrderRepo.GetOrderStatuses()).Select(orderStatus =>
            {
                return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = order.OrderStatusId == orderStatus.Id };
            });
            var data = new UpdateOrderStatusModel
            {

                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    data.OrderStatusList = (await _userOrderRepo.GetOrderStatuses()).Select(orderStatus =>
                    {
                        return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = orderStatus.Id == data.OrderStatusId };
                    });

                    return View(data);
                }
                await _userOrderRepo.ChangeOrderStatus(data);
                TempData["msg"] = "Updated successfully";
            }
            catch (Exception ex)
            {
                // catch exception here
                TempData["msg"] = "Something went wrong";
            }
            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }


    }
}
