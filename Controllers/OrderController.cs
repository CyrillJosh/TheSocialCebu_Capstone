using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Models.OrderClasses;
using TheSocialCebu_Capstone.ViewModels;

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

        public IActionResult Orders()
        {
            var date = DateOnly.Parse(DateTime.Now.ToString("MMMM dd, yyyy")); 
            var orders = _context.Orders.Include(x => x.OrderItems)
                .ThenInclude(x => x.Prod).ThenInclude(x => x.Subcategory).Include(x => x.Table)
                .Where(x => x.TableId == HttpContext.Session.GetString("Table") &&
                x.Paid == false); 
            return View(orders.ToList());
        }

        public IActionResult MyOrders()
        {
            var id = HttpContext.Session.GetString("Table");

            var table = _context.Tables
                .Include(x => x.Orders)
                .ThenInclude(x => x.OrderItems)
                .FirstOrDefault(x => x.Id == id);

            if (table == null)
                return NotFound();

            var orderItems = _context.OrderItems
                .Where(oi => oi.Order.Table.Id == id)
                .GroupBy(oi => new { oi.Prod.ProdId, oi.Prod.ProdName, oi.Prod.Price })
                .Select(g => new OrderitemSummary
                {
                    ProdId = g.Key.ProdId,
                    ProdName = g.Key.ProdName,
                    TotalQty = g.Sum(x => x.Qty),
                    Price = g.Key.Price,
                    TotalAmount = g.Sum(x => x.Qty * g.Key.Price),
                    CombinedInstructions = string.Join("; ", g.Select(x => x.Instructions))
                })
                .OrderByDescending(x => x.TotalQty)
                .ToList();

            return View(orderItems);
        }

        public IActionResult Index()
        {
            var orders = GetOrders();
            return View(orders);
        }
        public IActionResult AddToCart(string id, int qty)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProdId == id);
            if (product == null || !product.Availability) return NotFound();

            var orders = GetOrders();
            var existingItem = orders.FirstOrDefault(o => o.ProdId == id);
            if (existingItem != null)
            {
                existingItem.Qty+= qty;
            }
            else
            {
                orders.Add(new OrderItem
                {
                    OrderItemId = Guid.NewGuid().ToString(),
                    Qty = qty,
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

        public JsonResult ConfirmOrder(string id)
            {
            var orderItems = HttpContext.Session.GetString("Orders");
            if(orderItems == null)
                return Json(new {message = "Invalid!"});

            var items = JsonSerializer.Deserialize<List<OrderItem>>(orderItems);
            foreach (var item in items)
            {
                _context.Attach(item.Prod); 
            }
            Order order = new()
            {
                OrderId = Guid.NewGuid().ToString(),
                CreatedAt = DateOnly.Parse(DateTime.Now.ToString("MMMM dd, yyyy")),
                Status = true,
                TableId = id,
                Billings = null,
                OrderItems = items,
                Table = _context.Tables.FirstOrDefault(x => x.Id == id)
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            HttpContext.Session.Remove("Orders"); 
            _context.SaveChanges();

            return Json(new {message = "Confirming"});
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
