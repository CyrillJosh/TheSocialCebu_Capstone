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
            var table = _context.Tables.ToList();
            var orderitems = _context.OrderItems.Include(x => x.Order).ThenInclude(x => x.Session).ThenInclude(x => x.Table).Include(x => x.Prod).ToList();
            var vm = new List<BillingVM>();
            foreach (var item in table)
            {
                decimal subtotal = (decimal)orderitems.Sum(x => x.Quantity * x.Prod.Price);
                decimal tax = (decimal)orderitems.Sum(x => x.Quantity * x.Prod.Price) * (decimal)0.12;
                decimal servicecharge = (decimal)orderitems.Sum(x => x.Quantity * x.Prod.Price) * (decimal)0.10;
                vm.Add(new BillingVM()
                {
                    Table = item,
                    OrderItems = orderitems.Where(x => x.Order.Session.TableId == item.TableId)
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
                    Total = subtotal + tax + servicecharge
                });
            }
            return View(vm);
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
