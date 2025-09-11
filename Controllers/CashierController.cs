using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.ViewModels;

namespace TheSocialCebu_Capstone.Controllers
{
    public class CashierController : Controller
    {
        private readonly MyDBContext _context;
        public CashierController(MyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var table = _context.Tables.Where(x => x.Orders.Any(y => y.OrderStatusId == 4)).ToList();
            var orderitems = _context.OrderItems.Include(x => x.Order).ThenInclude(x => x.Table).Include(x => x.Prod).ToList();
            var orders = _context.Orders.Include(x => x.Table).ToList();
            var vm = new List<BillingVM>();
            foreach (var item in table)
            {
                decimal subtotal = (decimal)orderitems.Sum(x => x.Quantity * x.Prod.Price) / (decimal)1.12;
                decimal tax = (decimal)orderitems.Sum(x => x.Quantity * x.Prod.Price) - subtotal;
                decimal servicecharge = (decimal)orderitems.Sum(x => x.Quantity * x.Prod.Price) * (decimal)0.10;
                vm.Add(new BillingVM()
                {
                    Table = item,
                    OrderItems = orderitems.Where(x => x.Order.TableId == item.TableId)
                    .GroupBy(x => x.ProdId)
                    .Select(x => new OrderItem
                    {
                        ProdId = x.Key,
                        Prod = x.First().Prod,
                        Quantity = x.Sum(y => y.Quantity)
                    }),
                    Subtotal = subtotal,
                    Tax = tax,
                    ServiceCharge = servicecharge,
                    Total = subtotal + tax + servicecharge,
                    Orders = orders.Where(x => x.TableId == item.TableId).ToList()
                });
            }
            return View(vm);
        }

        public JsonResult PayBill(string tableid)
        {
            var table = _context.Tables.FirstOrDefault(x => x.TableId == tableid);
            if (table == null)
                return Json(new { message = "Error" });

            table.TableStatusId = 1;
            _context.Update(table);
            _context.SaveChanges();
            return Json(new {message = "Success"});
        }

        //public IActionResult PayBill(string tableid, string orderid)
        //{
        //    var table = _context.Tables.FirstOrDefault(x => x.TableId == tableid);
        //    var order = 
        //    var vm = new List<BillingVM>()
        //    {

        //    }
        //    return Json(new {content = vm });
        //}
    }
}
