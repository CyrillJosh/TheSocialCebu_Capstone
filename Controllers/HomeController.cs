using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;

namespace TheSocialCebu_Capstone.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDBContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyDBContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.Include(x => x.SubCategories).ThenInclude(x => x.Products).ToList();
            return View(categories);
        }
    }
}
