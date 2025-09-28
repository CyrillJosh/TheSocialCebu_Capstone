using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Hubs;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.Controllers
{
    public class OrderController : Controller
    {
        //Fields
        private readonly MyDBContext _context;
        private readonly IHubContext<ConnectorHub> _hub;

        public OrderController(MyDBContext context, IHubContext<ConnectorHub> hub)
        {
            _context = context;
            _hub = hub;
        }
        //Set session
        public IActionResult Table(string id)
        {
            var table = _context.Tables.FirstOrDefault(x => x.TableId == id);
            if (table == null) return NotFound();
            if (table.TableStatusId == 5) return NotFound();

            HttpContext.Session.SetString("Table", id);
            table.TableStatusId = table.TableStatusId == 1 ? 2 : table.TableStatusId;

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
            if (string.IsNullOrEmpty(tableId) || !_context.Tables.Any(x => x.TableId == tableId) || string.IsNullOrEmpty(category))
                return NotFound();

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
                return Json(new {success = false, message = "Error: No product found" });
            return Json(new
            {
                success = true,
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

            var orders = _context.Orders
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Prod)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.OrderItemStatus)
                .Where(x => x.TableId == tableId && x.OrderStatusId < 5).ToList();

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
            return Json(new { success = true, orders, message = "Success" });
        }
        public IActionResult AddToCart(string id, int qty, string ins)
        {
            var tableid = HttpContext.Session.GetString("Table").ToString();
            if(string.IsNullOrEmpty(tableid))
                return Json(new { success = false, message = "Error: No active session found." });
            var table = _context.Tables.FirstOrDefault(x => x.TableId == tableid);
            if(table == null || table.TableStatusId >= 3)
            {
                return Json(new {success = false, message = "Error: Cannot add new orders."});
            }

            var product = _context.Products.FirstOrDefault(x => x.ProdId == id);
            if (product == null || !product.Availability) return Json(new { success = false, message = "Error: No product found." });

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
            return Json(new {success = true, orders, message = "Success" });
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
            return Json(new { success = true, orders, message = "Success" });
        }

        public async Task<JsonResult> ConfirmCart(string tableId)
        {
            if (tableId == null)
            {
                return Json(new { success = false, message = "Error: No active session found." });
            }

            var orderItems = HttpContext.Session.GetString("Orders");
            if (orderItems == null)
                return Json(new { success = false, message = "Error: No items in cart" });

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
                OrderStatusId = 1,
                OrderItems = items,
                OrderNumber = _context.Orders.Count(x => x.TableId == tableId) + 1
            };

            _context.Orders.Add(order);
            HttpContext.Session.Remove("Orders");
            _context.SaveChanges();

            var orderDto = new
            {
                order.OrderId,
                order.OrderNumber,
                order.CreatedAt,
                Table = new
                {
                    order.Table.TableNumber
                },
                OrderStatus = new
                {
                    order.OrderStatusId,
                    StatusName = "To Accept" // or fetch from your context if needed
                },
                OrderItems = order.OrderItems.Select(x => new
                {
                    x.OrderItemId,
                    x.Quantity,
                    Instructions = x.Instructions ?? "",
                    Prod = new
                    {
                        x.Prod.ProdName,
                        Subcategory = new { x.Prod.SubcategoryId, x.Prod.Subcategory.SubcategoryName }
                    },
                    OrderItemStatus = new { x.OrderItemStatusId, StatusName = x.OrderItemStatus.StatusName }
                }).ToList()
            };

            await _hub.Clients.All.SendAsync("NewOrder", orderDto);

            return Json(new { success = true, message = "Success" });
        }
        #endregion


        public JsonResult RequestBill(string tableid)
        {
            if (string.IsNullOrEmpty(tableid))
                return Json(new { success = false, message = "Error", details = "Invalid table id" });

            var table = _context.Tables
                .Include(t => t.Orders)
                .FirstOrDefault(x => x.TableId == tableid);
            if (table.TableStatusId == 3) 
                return Json(new { success = false, message = "Error: Bill already requested"});
            if (table == null)
                return Json(new { success = false, message = "Error: Table not found!"});
            if (!table.Orders.Any())
                return Json(new { success = false, message = "Error: No orders available!"});

            // Update order statuses
            foreach (var order in table.Orders)
            {
                if (order.OrderStatusId < 4) 
                    order.OrderStatusId = 4; 
            }

            // Update table status
            table.TableStatusId = 3; 

            _context.Update(table);
            _context.SaveChanges();

            return Json(new { success = true, message = "Requesting bill"});
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
    }
}
