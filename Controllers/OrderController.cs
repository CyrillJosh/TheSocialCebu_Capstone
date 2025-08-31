using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
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

        #region Customer
        //Digital Menu
        public IActionResult Menu(string category)
        {
            var tableId = HttpContext.Session.GetString("Table");
            if (string.IsNullOrEmpty(tableId) || !_context.Tables.Any(x => x.TableId == tableId))
                return NotFound();
            if(string.IsNullOrEmpty(category))
                return NotFound();
            var sessionId = HttpContext.Session.GetString("SessionId");
            TableSession? currentSession = null;

            if (!string.IsNullOrEmpty(sessionId))
            {
                // Find active session by SessionId and TableId
                currentSession = _context.TableSessions
                                    .FirstOrDefault(s => s.SessionId == sessionId && s.TableId == tableId && s.EndedAt == null);
            }

            // If no active session, create a new one
            if (currentSession == null)
            {
                currentSession = new TableSession { TableId = tableId, StartedAt = DateTime.Now };
                _context.TableSessions.Add(currentSession);
                _context.SaveChanges();
                HttpContext.Session.SetString("SessionId", currentSession.SessionId);
            }
            // --- END REFACTOR ---

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
            var sessionsWithOrders = _context.TableSessions
                                .Include(ts => ts.Table)
                                .Include(ts => ts.Orders)
                                    .ThenInclude(o => o.OrderStatus) // Include the main order status
                                .Include(ts => ts.Orders)
                                    .ThenInclude(o => o.OrderItems)
                                        .ThenInclude(oi => oi.Prod) // Include product details
                                .Include(ts => ts.Orders)
                                    .ThenInclude(o => o.OrderItems)
                                        .ThenInclude(oi => oi.OrderItemStatus) // Include the item status
                                .Where(ts => ts.TableId == tableId && ts.EndedAt == null)
                                .ToList();

            return View(sessionsWithOrders);
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
            var sessionId = HttpContext.Session.GetString("SessionId");
            if (sessionId == null)
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
                SessionId = sessionId,
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


        #region Kitchen
        public IActionResult Kitchen()
        {
            var orders = _context.Orders.Include(x => x.OrderStatus).Include(x => x.Session).ThenInclude(x => x.Table).Include(x => x.OrderItems).ThenInclude(x => x.Prod).Include(x => x.OrderItems).ThenInclude(x => x.OrderItemStatus).OrderBy(x => x.CreatedAt).ToList(); 
            return View(orders);
        }
        public IActionResult ConfirmOrder(string orderid)
        {
            var order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.OrderId == orderid);
            foreach (var item in order.OrderItems)
            {
                item.OrderItemStatusId = 2; // Set to 'In Progress'
            }
            order.OrderStatusId = 2; // Set to 'In Progress'
            _context.Update(order);
            _context.SaveChanges();

            return Json(new { message = "Success" });

            //var itemsid = id.Split(",");
            //var orderitems = _context.OrderItems.Include(x => x.Order).Include(x => x.OrderItemStatus) .ToList();
            //var updateItems = new List<OrderItem>();
            //foreach (var itemid in itemsid)
            //{
            //    var orderitem = orderitems.FirstOrDefault(x => x.OrderItemId == itemid);
            //    if (orderitem != null)
            //    {
            //        orderitem.OrderItemStatusId = 2; 
            //        updateItems.Add(orderitem);
            //    }
            //}

            //_context.UpdateRange(updateItems);
            //var order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.OrderId == orderid);
            //if(order.OrderItems.All(x => x.OrderItemStatusId == 2))
            //{
            //    order.OrderStatusId = 2;
            //}
            //_context.Update(order);
            //_context.SaveChanges();
            //return Json(new { message = "Success" });
            //if (id == null)
            //{
            //    return Json(new { message = "Error" });
            //}
            //var order = _context.Orders.FirstOrDefault(x => x.OrderId == id);
            //order.OrderStatusId = 2;
            //_context.Update(order);
            //_context.SaveChanges();
        }

        public IActionResult CompleteOrder(string id)
        {   
            //Addan pa for each item or tagsatagsa for each item
            if (id == null)
            {
                return Json(new { message = "Error" });
            }
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == id);
            order.OrderStatusId = 3;
            _context.Update(order);
            _context.SaveChanges();
            return Json(new { message = "Success" });
        }

        public IActionResult UpdateMenu()
        {
            //UI for updating menu items or reuse menu/index add condition for role set
            var products = _context.Products.Where(x => x.Availability == true).ToList();
            return View(products);
        }
        #endregion

        #region Manager
        public IActionResult Orders()
        {
            var date = DateOnly.Parse(DateTime.Now.ToString("MMMM dd, yyyy"));
            var sessionsWithOrders = _context.TableSessions
                         .Include(ts => ts.Table)
                         .Include(ts => ts.Orders)
                         .ThenInclude(o => o.OrderItems)
                         .ThenInclude(oi => oi.Prod)
                         .Where(ts => ts.EndedAt == null && ts.Orders.Any()) // Only active sessions with at least one order
                         .ToList();
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
            return View(sessionsWithOrders);
        }
        #endregion




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
