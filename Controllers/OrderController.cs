using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models.MenuClasses;
using TheSocialCebu_Capstone.Models.OrderClasses;
using TheSocialCebu_Capstone.ViewModels;

namespace TheSocialCebu_Capstone.Controllers
{
    public class OrderController : Controller
    {
        //Fields
        private readonly MyDBContext _context;
        private List<Category> Categories;
        private List<SubCategory> Subcategories;
        public OrderController(MyDBContext context)
        {
            _context = context;
        }
        //Set session
        public IActionResult Table(string id)
        {
            HttpContext.Session.SetString("Table", id);
            return RedirectToAction("Index","Home");
        }
        [HttpGet]

        //Digital Menu
        public IActionResult Menu(string category)
        {
            var table = HttpContext.Session.GetString("Table");
            if (string.IsNullOrEmpty(table) || !_context.Tables.Any(x => x.Id == table))
                return NotFound();
            if(string.IsNullOrEmpty(category))
                return NotFound();

            var subcat = _context.SubCategories.Where(x => x.CategoryId == category).Include(x => x.Products).ToList();


            if (HttpContext.Session.GetString("Table") == null || HttpContext.Session.GetString("Order") == null)
            {
                //Check for table this table orders
                var orders = _context.OrderItems.Where(x => x.Order.TableId == table /* && x.Paid == false*/).ToList();
                if (orders.Any())
                {
                    HttpContext.Session.SetString("Order", orders.First().OrderId);
                }
                else
                {
                    HttpContext.Session.SetString("Order", Guid.NewGuid().ToString());
                }
            }

            return View(subcat);
        }

        //Preview Product
        public IActionResult Preview(string id)
        {
            var product = _context.Products.Where(x => x.Availability == true).FirstOrDefault(x => x.ProdId == id);
            if (product == null)
                return NotFound();
            return Json(new
            {
                prodId = product.ProdId,
                prodName = product.ProdName,
                description = product.Description,
                price = product.Price,
            });
        }
        public IActionResult Orders()
        {
            var date = DateOnly.Parse(DateTime.Now.ToString("MMMM dd, yyyy"));
            var orders = _context.Tables.Include(x => x.Orders).ThenInclude(x => x.OrderItems).ThenInclude(x => x.Prod).ToList();
            //    var orders = _context.Orders
            //        .Include(x => x.OrderItems)
            //        .ThenInclude(x => x.Prod)
            //        .ThenInclude(x => x.Subcategory)
            //        .Include(x => x.Table)
            //        .Where(x => x.Paid == false)
            //        .GroupBy(x=> new { x.TableId, x.OrderId, x.OrderItems })
            //        .Select(g => new Order
            //        {
            //            OrderId = g.Key.OrderId,
            //            TableId = g.Key.TableId,
            //            OrderItems = g.Key.OrderItems.ToList()
            //        }).ToList(); 
            return View(orders);
        }

        public IActionResult ConfirmOrder(string id, string status)
        {
            if (id == null)
            {
                return Json(new { message = "Error" });
            }
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == id);
            order.Status = false;
            _context.Update(order);
            _context.SaveChanges();
            return Json(new { message = "OK" });
        }

        public IActionResult MyOrders()
        {
            var id = HttpContext.Session.GetString("Table");
            var orderitems = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Prod).Where(x => x.TableId == id).OrderByDescending(x=> x.Status).ToList();
            //var table = _context.Tables
            //    .Include(x => x.Orders)
            //    .ThenInclude(x => x.OrderItems)
            //    .FirstOrDefault(x => x.Id == id);

            //if (table == null)
            //    return NotFound();
            ////Adjust
            //var orderItems = _context.OrderItems
            //    .Where(oi => oi.Order.Table.Id == id)
            //    .GroupBy(oi => new { oi.Prod.ProdId, oi.Prod.ProdName, oi.Prod.Price })
            //    .Select(g => new OrderitemSummary
            //    {
            //        ProdId = g.Key.ProdId,
            //        ProdName = g.Key.ProdName,
            //        TotalQty = g.Sum(x => x.Qty),
            //        Price = g.Key.Price,
            //        TotalAmount = g.Sum(x => x.Qty * g.Key.Price),
            //        CombinedInstructions = string.Join("; ", g.Select(x => x.Instructions))
            //    })
            //    .OrderByDescending(x => x.TotalQty)
            //    .ToList();

            return View(orderitems);
        }

        public IActionResult Cart()
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

        public JsonResult ConfirmCart(string id)
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
                Status = false,
                TableId = id,
                Billings = null,
                OrderItems = items,
                Table = _context.Tables.FirstOrDefault(x => x.Id == id)
            };
            _context.Orders.Add(order);
            HttpContext.Session.Remove("Orders"); 
            _context.SaveChanges();

            return Json(new {message = "Confirming"});
        }

        public IActionResult Kitchen()
        {
            var orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Prod).ToList(); 
            return View(orders);
        }

        //
        //Custom Methods
        //

        // Get Orders
        private List<OrderItem> GetOrders()
        {
            var orders = HttpContext.Session.GetString("Orders");
            if (orders != null)
                return JsonSerializer.Deserialize<List<OrderItem>>(orders);
            return new List<OrderItem>();
        }

        //Save cart
        private void SaveCart(List<OrderItem> orders)
        {
            HttpContext.Session.SetString("Orders", JsonSerializer.Serialize(orders));
        }

        private void PopulateCategories()
        {
            Categories = _context.Categories.ToList();
            Subcategories = _context.SubCategories.Include(x => x.Category).ToList();
        }
    }
}
