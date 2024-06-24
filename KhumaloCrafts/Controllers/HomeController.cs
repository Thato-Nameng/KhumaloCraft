using KhumaloCrafts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using KhumaloCrafts.ViewModels;
using KhumaloCrafts.Repo;

namespace KhumaloCrafts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepo _homeRepo;

        public HomeController(ILogger<HomeController> logger, IHomeRepo homeRepo)
        {
            _homeRepo = homeRepo;
            _logger = logger;
        }

        public async Task<IActionResult> MyWork(string sterm = "", int categoryId = 0)
        {
            IEnumerable<Craft> crafts = await _homeRepo.GetCrafts(sterm, categoryId);
            IEnumerable<Category> categories = await _homeRepo.Categories();
            CraftDisplayModel craftModel = new CraftDisplayModel
            {
                Crafts = crafts,
                Categories = categories,
                STerm = sterm,
                CategoryId = categoryId
            };

            return View(craftModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

