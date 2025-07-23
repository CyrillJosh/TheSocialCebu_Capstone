using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models.MenuClasses;
using TheSocialCebu_Capstone.Models.OrderClasses;

namespace TheSocialCebu_Capstone.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyDBContext _context;
        public IActionResult Index()
        {
            var orders = GetOrders();
            return View(orders);
        }
        public IActionResult AddToCart(string id, string instructions)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProdId == id);
            if (product == null || !product.Availability) return NotFound();

            var orders = GetOrders();
            var existingItem = orders.FirstOrDefault(o => o.ProdId == id);
            if (existingItem != null)
            {
                existingItem.Qty++;
            }
            else
            {
                orders.Add(new OrderItem
                {
                    Qty = 1,
                    Instructions = instructions,
                    ProdId = id,
                    OrderId = HttpContext.Session.GetString("Order"),

                });
            }

            SaveCart(orders);
            return Json(orders);
        }

        public IActionResult RemoveFromCart(string id)
        {
            var orders = GetOrders();
            var item = orders.FirstOrDefault(o => o.ProdId == id);
            if (item != null)
            {
                orders.Remove(item);
                SaveCart(orders);
            }
            return Json(orders);
        }

        private List<OrderItem> GetOrders()
        {
            var orders = HttpContext.Session.GetString("Orders");
            if (orders != null)
                return JsonSerializer.Deserialize<List<OrderItem>>(orders);
            return new List<OrderItem>();
        }

        private void SaveCart(List<OrderItem> orders)
        {
            HttpContext.Session.SetString("Orders", JsonSerializer.Serialize(orders));
        }

    }
}
