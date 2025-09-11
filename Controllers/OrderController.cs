using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;

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
            var table = _context.Tables.FirstOrDefault(x => x.TableId == id);
            table.TableStatusId = 2;
            _context.Update(table);
            _context.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        #region Customer
        [HttpGet]
        //Digital Menu
        public IActionResult Menu(string category)
        {
            var tableId = HttpContext.Session.GetString("Table");
            if (string.IsNullOrEmpty(tableId) || !_context.Tables.Any(x => x.TableId == tableId))
                return NotFound();
            if(string.IsNullOrEmpty(category))
                return NotFound();


            //var sessionId = HttpContext.Session.GetString("SessionId");
            //TableSession? currentSession = null;

            //if (!string.IsNullOrEmpty(sessionId))
            //{
            //    currentSession = _context.TableSessions
            //                        .FirstOrDefault(s => s.SessionId == sessionId && s.TableId == tableId && s.EndedAt == null);
            //}

            //// If no active session, create a new one
            //if (currentSession == null)
            //{
            //    currentSession = new TableSession { TableId = tableId, StartedAt = DateTime.Now };
            //    _context.TableSessions.Add(currentSession);
            //    _context.SaveChanges();
            //    HttpContext.Session.SetString("SessionId", currentSession.SessionId);
            //}
            //// --- END REFACTOR ---

            var subcat = _context.SubCategories
                .Where(x => x.CategoryId == category)
                .Include(x => x.Products.Where(p => p.Availability)) // Only show available products
                .Include(x => x.Category)
                .ToList();

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
        //Customer Orders
        public IActionResult MyOrders()
        {
            var tableId = HttpContext.Session.GetString("Table");
            // --- REFACTOR: Use a single, efficient query to get aggregated order items ---
            //var sessionsWithOrders = _context.TableSessions
            //                    .Include(ts => ts.Table)
            //                    .Include(ts => ts.Orders)
            //                        .ThenInclude(o => o.OrderStatus) // Include the main order status
            //                    .Include(ts => ts.Orders)
            //                        .ThenInclude(o => o.OrderItems)
            //                            .ThenInclude(oi => oi.Prod) // Include product details
            //                    .Include(ts => ts.Orders)
            //                        .ThenInclude(o => o.OrderItems)
            //                            .ThenInclude(oi => oi.OrderItemStatus) // Include the item status
            //                    .Where(ts => ts.TableId == tableId && ts.EndedAt == null && ts.Orders.Count > 0)
            //                    .ToList();

            var orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Prod).Include(x => x.OrderItems).ThenInclude(x => x.OrderItemStatus).Where(x => x.TableId == tableId).ToList();
            return View(orders);
        }


        //Cart Operations
        public IActionResult Cart()
        {
            var orders = GetOrders();
            return View(orders);
        }
        public IActionResult UpdateQuantity(string id, string type)
        {
            var orders = GetOrders();
            switch (type) {
                case "Add":
                    orders.FirstOrDefault(x => x.OrderItemId == id).Quantity++;
                    break;
                case "Minus":
                    var order = orders.FirstOrDefault(x => x.OrderItemId == id);
                    if (order.Quantity - 1 == 0)
                        order.Quantity = 1;
                    else
                        order.Quantity--;
                        break;
            }
            SaveCart(orders);
            return Json(new { orders = orders, message = "Success" });
        }
        public IActionResult AddToCart(string id, int qty, string ins)
        {
            var tableid = HttpContext.Session.GetString("Table").ToString();
            if(string.IsNullOrEmpty(tableid))
                return Json(new {message = "Error" });
            var table = _context.Tables.FirstOrDefault(x => x.TableId == tableid);
            if(table == null || table.TableStatusId == 3)
            {
                return Json(new {message = "Error cannot add new orders"});
            }

            var product = _context.Products.FirstOrDefault(x => x.ProdId == id);
            if (product == null || !product.Availability) return Json(new { message = "Error" });

            var orders = GetOrders();
            var existingItem = orders.FirstOrDefault(o => o.ProdId == id);
            if (existingItem != null)
            {
                existingItem.Quantity += qty;
            }
            else
            {
                orders.Add(new OrderItem
                {
                    OrderItemId = Guid.NewGuid().ToString(),
                    Quantity = qty,
                    Instructions = ins == null? "": ins,
                    ProdId = id,
                    Prod = product,
                });
            }

            SaveCart(orders);
            return Json(new { orders = orders, message = "Success" });
        }

        public IActionResult RemoveFromCart(string id)
        {
            var orders = GetOrders();
            var item = orders.FirstOrDefault(o => o.OrderItemId == id);
            if (item != null)
            {
                orders.Remove(item);
                SaveCart(orders);
            }
            return Json(new { orders = orders, message = "Success" });
        }

        public JsonResult ConfirmCart()
        {
            var tableId = HttpContext.Session.GetString("Table");
            if (tableId == null)
            {
                return Json(new { message = "Error: No active session found." });
            }

            var orderItems = HttpContext.Session.GetString("Orders");
            if (orderItems == null)
                return Json(new { message = "Error: No items in cart" });

            var items = JsonSerializer.Deserialize<List<OrderItem>>(orderItems);

            foreach (var item in items)
            {
                item.OrderItemStatusId = 1;

                if (item.Prod != null)
                    _context.Attach(item.Prod);
            }

            Order order = new()
            {
                OrderId = Guid.NewGuid().ToString(),
                TableId = tableId,
                CreatedAt = DateTime.Now,
                OrderStatusId = 1, // Assuming 1 is the 'Pending' status ID
                OrderItems = items
            };

            _context.Orders.Add(order);
            HttpContext.Session.Remove("Orders");
            _context.SaveChanges();

            return Json(new { message = "Success" });
        }
        #endregion


        public JsonResult RequestBill(string tableid)
        {
            if (string.IsNullOrEmpty(tableid) || !_context.Tables.Any(x => x.TableId == tableid))
                return Json(new { message = "Error" });
            var orders = _context.Orders.Where(x => x.TableId == tableid).ToList();
            foreach (var order in orders)
            {
                order.OrderStatusId = 4;
            }
            var table = _context.Tables.FirstOrDefault(x => x.TableId == tableid);
            if (table == null)
                return Json(new { message = "Error" });

            table.TableStatusId = 3;
            _context.UpdateRange(orders);
            _context.Update(table);
            _context.SaveChanges();

            return Json(new { message = "Requesting" });
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
