using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Models.OrderClasses;

namespace TheSocialCebu_Capstone.Controllers
{
    public class OrderController : Controller
    {
        //Fields
        private readonly MyDBContext _context;

        public OrderController(MyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var orders = GetOrders();
            return View(orders);
        }
        public IActionResult AddToCart(string id/*, string instructions*/)
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
                    OrderItemId = Guid.NewGuid().ToString(),
                    Qty = 1,
                    Instructions = "DEBUG",
                    ProdId = id,
                    OrderId = HttpContext.Session.GetString("Order"),
                    Prod = product,
                    Order = new Order()
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

        public JsonResult Checkout(string id)
        {
            var orderItems = HttpContext.Session.GetString("Orders");
            if (orderItems == null)
                return Json(new { message = "Invalid!" });
            var items = JsonSerializer.Deserialize<List<OrderItem>>(orderItems);
            foreach (var item in items)
            {
                _context.Attach(item.Prod);
            }
            Order order = new()
            {
                OrderId = Guid.NewGuid().ToString(),
                CreatedAt = DateOnly.Parse(DateTime.Now.ToString("MMMM dd, yyyy")),
                Status = "0",
                TableId = id,
                Billings = null,
                OrderItems = items,
                Table = null
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            HttpContext.Session.Remove("Orders");
            return Json(new { message = "Success!" });
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
