using KhumaloCrafts.Constants;
using KhumaloCrafts.ViewModels;
using KhumaloCrafts.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCrafts.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepo _stockRepo;

        public StockController(IStockRepo stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stocks = await _stockRepo.GetStocks(sTerm);
            return View(stocks);
        }
        [HttpPost]
        public async Task<IActionResult> ManangeStock(int craftId)
        {
            var existingStock = await _stockRepo.GetStockByCraftId(craftId);
            var stock = new StockDTO
            {
                CraftId = craftId,
                Availability = existingStock != null
            ? existingStock.Availability : 0
            };
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManangeStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stockRepo.ManageStock(stock);
                TempData["successMessage"] = "Stock is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}