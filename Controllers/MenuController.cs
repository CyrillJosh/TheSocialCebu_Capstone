using Microsoft.AspNetCore.Mvc;

namespace TheSocialCebu_Capstone.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
