using KhumaloCrafts.Data;
using KhumaloCrafts.Models;
using KhumaloCrafts.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KhumaloCrafts.Repo
{
    public class CartRepo : ICartRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartRepo> _logger;

        public CartRepo(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, ILogger<CartRepo> logger)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<int> AddItem(int craftId, int avail)
        {
            string userId = GetUserId();
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not logged in");
                }

                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                    await _db.SaveChangesAsync();
                }

                var cartItem = await _db.CartDetails.FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.CraftId == craftId);
                if (cartItem != null)
                {
                    cartItem.Availability += avail;
                }
                else
                {
                    var craft = await _db.Crafts.FindAsync(craftId);
                    if (craft == null)
                    {
                        throw new InvalidOperationException("Craft not found");
                    }

                    cartItem = new CartDetail
                    {
                        CraftId = craftId,
                        ShoppingCartId = cart.Id,
                        Availability = avail,
                        UnitPrice = craft.ProductPrice
                    };
                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Item added to cart successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding item to cart: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }

            return await GetCartItemCount(userId);
        }

        public async Task<int> RemoveItem(int craftId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not logged in");
                }

                var cart = await GetCart(userId);
                if (cart == null)
                {
                    throw new InvalidOperationException("Invalid cart");
                }

                var cartItem = await _db.CartDetails.FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.CraftId == craftId);
                if (cartItem == null)
                {
                    throw new InvalidOperationException("No items in cart");
                }
                else if (cartItem.Availability == 1)
                {
                    _db.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Availability--;
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation("Item removed from cart successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing item from cart: {ex.Message}");
                throw;
            }

            return await GetCartItemCount(userId);
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("Invalid userId");
            }

            var shoppingCart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Craft)
                .ThenInclude(a => a.Stock)
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Craft)
                .ThenInclude(a => a.Category)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();

            return shoppingCart;
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            return await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails on cart.Id equals cartDetail.ShoppingCartId
                              where cart.UserId == userId && !cart.IsDeleted
                              select new { cartDetail.Id })
                             .ToListAsync();

            return data.Count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not logged in");
                }
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    throw new InvalidOperationException("Cart is invalid");
                }

                var cartDetails = await _db.CartDetails.Where(cd => cd.ShoppingCartId == cart.Id).ToListAsync();
                if (!cartDetails.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }

                var pendingStatus = await _db.orderStatuses.FirstOrDefaultAsync(s => s.StatusName == "Pending");
                if (pendingStatus == null)
                {
                    throw new InvalidOperationException("Order status 'Pending' not found");
                }

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = pendingStatus.StatusId,
                    IsDeleted = false,
                    Address = model.Address,
                    Email = model.Email,
                    IsPaid = false,
                    MobileNumber = model.MobileNumber,
                    Name = model.Name,
                    PaymentMethod = model.PaymentMethod
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                foreach (var item in cartDetails)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        CraftId = item.CraftId,
                        Availability = item.Availability,
                        UnitPrice = item.UnitPrice
                    };

                    _db.OrderDetails.Add(orderDetail);

                    var stock = await _db.Stocks.FirstOrDefaultAsync(s => s.CraftId == item.CraftId);
                    if (stock == null)
                    {
                        throw new InvalidOperationException($"Stock for craft ID {item.CraftId} not found");
                    }

                    if (item.Availability > stock.Availability)
                    {
                        throw new InvalidOperationException($"Insufficient stock for craft ID {item.CraftId}");
                    }

                    stock.Availability -= item.Availability;
                }

                _db.CartDetails.RemoveRange(cartDetails);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Checkout completed successfully.");

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error during checkout: {ex.Message}");
                throw;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            return _userManager.GetUserId(principal);
        }
    }
}
