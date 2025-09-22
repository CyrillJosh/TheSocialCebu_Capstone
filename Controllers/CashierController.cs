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
            // Only tables that requested a bill (TableStatusId == 3)
            var tables = _context.Tables
                .Where(x => x.TableStatusId == 3 || x.TableStatusId == 4)
                .ToList();

            var orderitems = _context.OrderItems
                .Include(x => x.Order)
                .ThenInclude(x => x.Table)
                .Include(x => x.Prod)
                .ToList();

            var orders = _context.Orders
                .Include(x => x.Table)
                .ToList();

            var vm = new List<BillingVM>();

            foreach (var item in tables)
            {
                var tableOrders = orderitems.Where(x => x.Order.TableId == item.TableId).ToList();

                decimal temp = (decimal)tableOrders.Sum(x => x.Quantity * x.Prod.Price);
                decimal subtotal = temp / 1.12m;
                decimal tax = temp - subtotal;
                decimal servicecharge = temp * 0.10m;

                vm.Add(new BillingVM()
                {
                    Table = item,
                    OrderItems = tableOrders
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

        [HttpGet]
        public JsonResult GenerateBill(string tableid)
        {
            var table = _context.Tables
                .Include(t => t.Orders.Where(o => o.OrderStatusId == 4)) // only orders that requested bill
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Prod)
                .FirstOrDefault(t => t.TableId == tableid);

            if (table == null || !table.Orders.Any())
                return Json(new {success = false, message = "Error", detail = "No billable orders found." });

            // Compute totals
            decimal total = (decimal)table.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.Prod.Price));
            decimal subtotal = total / 1.12m;
            decimal tax = total - subtotal;
            decimal servicecharge = total * 0.10m;
            decimal grandTotal = subtotal + tax + servicecharge;

            // Create new bill
            var bill = new Billing
            {
                BillingId = Guid.NewGuid().ToString(),
                TableId = tableid,
                Subtotal = subtotal,
                VatAmount = tax,
                ServiceCharge = servicecharge,
                GrandTotal = grandTotal,
                BillingTime = DateTime.Now,
                Payment = null // No payment yet
            };

            _context.Billings.Add(bill);

            // Link all billable orders to this bill
            foreach (var order in table.Orders)
            {
                var bo = new BillingOrder
                {
                    BillingId = bill.BillingId,
                    OrderId = order.OrderId
                };
                _context.Add(bo);

                // Mark orders as "Billed" if you have such a status (optional)
                order.OrderStatusId = 4; // e.g. 4 = Billed / Waiting Payment
            }

            // Update table status (4 = Waiting for Payment)
            table.TableStatusId = 4;

            _context.SaveChanges();

            return Json(new { success = true, message = "Success", billId = bill.BillingId });
        }

        [HttpPost]
        public JsonResult PayBill(string tableid, decimal amount)
        {
            var bills = _context.Billings.ToList();
            var bill = _context.Billings
                .Include(b => b.Payment)
                .Include(b => b.Table)
                .Include(b => b.BillingOrders)
                .ThenInclude(b => b.Order)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Prod)
                .FirstOrDefault(b => b.TableId == tableid); //No Payment yet
            //var bill = _context.Billings
            //    .Include(b => b.Table)
            //    .Include(b => b.BillingOrders)
            //    .ThenInclude(b => b.Order)
            //    .FirstOrDefault(b => b.TableId == tableid && !b.Payments.Any());

            if (bill.Payment != null)
                return Json(new { success = false, message = "Bill already paid" });

            if (bill.GrandTotal > amount)
                return Json(new { success = false, message = "Amount must be greater than the bill" });

            //Record payment
            var payment = new Payment
            {
                PaymentId = bill.BillingId, //Use the same ID as BillingId
                AmountPaid = amount,
                PaymentTime = DateTime.Now,
                PaymentNavigation = bill
            };

            //Close all orders under this bill
            foreach (var order in bill.BillingOrders.Select(x => x.Order))
            {
                order.OrderStatusId = 5; //Completed
            }

            //Reset table to Available
            bill.Table.TableStatusId = 1;

            _context.Payments.Add(payment);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                subtotal = bill.Subtotal,
                tax = bill.VatAmount,
                servicecharge = bill.ServiceCharge,
                total = bill.GrandTotal,
                change = amount - bill.GrandTotal,
                payment = new
                {
                    id = payment.PaymentId,
                    amountPaid = payment.AmountPaid,
                    time = payment.PaymentTime
                }
            });
        }

            //public JsonResult GenerateBIll(string tableid)
            //    {
            //    var table = _context.Tables.Include(x => x.Orders).FirstOrDefault(x => x.TableId == tableid);
            //    if (table == null)
            //        return Json(new { message = "Error" });

            //    foreach (var o in table.Orders)
            //    {
            //        o.OrderStatusId = 4;
            //    }
            //    table.TableStatusId = 4;
            //    _context.Update(table);
            //    _context.SaveChanges();
            //    return Json(new { message = "Success"/*, orders =  table.Orders*/ });
            //}

            //public JsonResult CalculatePayment(string tableid, decimal amount) {
            //    var t = _context.Tables.Include(x => x.Orders).ThenInclude(x => x.OrderItems).ThenInclude(x => x.Prod).FirstOrDefault(x => x.TableId == tableid);

            //    var total = t.Orders.Sum(x => x.OrderItems.Sum(y => y.Prod.Price * y.Quantity));
            //    decimal am = (decimal)(total - amount);
            //    return Json(new { message = "Success", amount = am });
            //}
    }
}
