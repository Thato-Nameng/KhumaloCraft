using KhumaloCrafts.Data;
using KhumaloCrafts.Models;
using KhumaloCrafts.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KhumaloCrafts.Repo
{
    public class StockRepo : IStockRepo
    {
        private readonly ApplicationDbContext _context;

        public StockRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockByCraftId(int craftId) => await _context.Stocks.FirstOrDefaultAsync(s => s.CraftId == craftId);

        public async Task ManageStock(StockDTO stockToManage)
        {
            var existingStock = await GetStockByCraftId(stockToManage.CraftId);
            if (existingStock is null)
            {
                var stock = new Stock { CraftId = stockToManage.CraftId, Availability = stockToManage.Availability };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Availability = stockToManage.Availability;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from craft in _context.Crafts
                                join stock in _context.Stocks
                                on craft.Id equals stock.CraftId
                                into craft_stock
                                from craftStock in craft_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || craft.CraftName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    CraftId = craft.Id,
                                    CraftName = craft.CraftName,
                                    Availability = craftStock == null ? 0 : craftStock.Availability
                                }
                                ).ToListAsync();
            return stocks;
        }
    }

    public interface IStockRepo
    {
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
        Task<Stock?> GetStockByCraftId(int craftId);
        Task ManageStock(StockDTO stockToManage);
    }
}