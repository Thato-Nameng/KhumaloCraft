using KhumaloCrafts.Data;
using KhumaloCrafts.Models;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System.IO;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KhumaloCrafts.Repo
{
    public class HomeRepo : IHomeRepo
    {
        private readonly ApplicationDbContext _db;

        public HomeRepo(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Categories.ToListAsync();
        }
        public async Task<IEnumerable<Craft>> GetCrafts(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Craft> crafts = await (from craft in _db.Crafts
                                               join category in _db.Categories
                                               on craft.CategoryId equals category.Id
                                               join stock in _db.Stocks
                                               on craft.Id equals stock.CraftId
                                               into craft_stocks
                                               from craftWithStock in craft_stocks.DefaultIfEmpty()
                                               where string.IsNullOrWhiteSpace(sTerm) || (craft != null && craft.CraftName.ToLower().StartsWith(sTerm))
                                               select new Craft
                                               {
                                                   Id = craft.Id,
                                                   Image = craft.Image,
                                                   CraftName = craft.CraftName,
                                                   Description = craft.Description,
                                                   CategoryId = craft.CategoryId,
                                                   ProductPrice = craft.ProductPrice,
                                                   Availability = craftWithStock == null ? 0 : craftWithStock.Availability
                                               }
                                               ).ToListAsync();
            if (categoryId > 0)
            {
                crafts = crafts.Where(a => a.CategoryId == categoryId).ToList();
            }

            return crafts;
        }
    }
}
