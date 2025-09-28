using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Hubs;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.Controllers
{
    public class KitchenController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IHubContext<ConnectorHub> _hub;
        public KitchenController(MyDBContext context, IHubContext<ConnectorHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.Include(x => x.OrderStatus).Include(x => x.Table).Include(x => x.OrderItems).ThenInclude(x => x.Prod).ThenInclude(x => x.Subcategory).Include(x => x.OrderItems).ThenInclude(x => x.OrderItemStatus).OrderBy(x => x.CreatedAt).ToList();
            return View(orders);
        }

        public IActionResult Menu()
        {
            var products = _context.Products.Include(p => p.Subcategory).ThenInclude(s => s.Category).OrderByDescending(o => o.Availability).ThenBy(o => o.ProdName).ToList();
            return View(products);
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders.Include(x => x.Table).Include(x => x.OrderItems).ThenInclude(x => x.Prod).ThenInclude(x => x.Subcategory).Include(x => x.OrderItems).OrderBy(x => x.CreatedAt).Where(x => x.OrderStatusId >= 3).ToList();
            return View(orders);
        }

        public IActionResult UpdateMenu(string id)
        {
            var prod = _context.Products.FirstOrDefault(x => x.ProdId == id);
            if(prod != null)
            {
                prod.Availability = !prod.Availability;
                _context.Update(prod);
                _context.SaveChanges();
                _hub.Clients.All.SendAsync("UpdateProductStatus", prod.ProdId, prod.Availability);
                return Json(new { message = "Success" });
            }
            return Json(new { message = "Error" });
        }

        public IActionResult ConfirmOrder(string orderid)
        {
            var order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.OrderId == orderid);
            foreach (var item in order.OrderItems)
            {
                item.OrderItemStatusId = 2; // Set to 'Preparing'
            }
            order.OrderStatusId = 2; // Set to 'In Progress'
            _context.Update(order);
            _context.SaveChanges();
            foreach(var item in order.OrderItems)
            {
                _hub.Clients.All.SendAsync("UpdateOrderStatus", item.OrderItemId, "Preparing");
            }

            return Json(new { message = "Success" });
        }

        public IActionResult ServeOrder(string orderid, string id)
        {

            if(string.IsNullOrEmpty(id))
                return Json(new { message = "Error" });
            var itemsid = id.Split(",");
            var orderitems = _context.OrderItems.Include(x => x.Order).Include(x => x.OrderItemStatus).ToList();
            var updateItems = new List<OrderItem>();
            if (orderitems == null || itemsid.All(x => string.IsNullOrEmpty(x)))
                return Json(new { message = "Error" });
            foreach (var itemid in itemsid)
            {
                var orderitem = orderitems.FirstOrDefault(x => x.OrderItemId == itemid && x.Order.OrderId == orderid);
                if (orderitem != null)
                {
                    orderitem.OrderItemStatusId = 3;
                    updateItems.Add(orderitem);
                }
            }

            _context.UpdateRange(updateItems);
            var order = _context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.OrderId == orderid);
            if (order.OrderItems.All(x => x.OrderItemStatusId == 3))
            {
                order.OrderStatusId = 3;
            }
            _context.Update(order);
            _context.SaveChanges();
            foreach (var item in updateItems)
            {
                _hub.Clients.All.SendAsync("UpdateOrderStatus", item.OrderItemId, "Served");
            }

            return Json(new { message = "Success" });
        }
    }
}
