using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                .Where(x => x.OrderStatusId < 5)
                .ToList();

            var vm = new List<BillingVM>();

            foreach (var item in tables)
            {
                var tableOrders = orderitems.Where(x => x.Order.TableId == item.TableId).Where(x => x.Order.OrderStatusId < 5).ToList();

                decimal temp = (decimal)tableOrders.Sum(x => x.Quantity * x.Prod.Price);
                decimal subtotal = temp / 1.12m;
                decimal tax = temp - subtotal;
                decimal servicecharge = temp * 0.10m;
                var disc = _context.DiscountTypes.ToList();
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
                    Orders = orders.Where(x => x.TableId == item.TableId).ToList(),
                    DiscountTypes = disc
                });
            }

            return View(vm);
        }

        [HttpGet]
        public JsonResult GenerateBill(string tableid, string discountid = null)
        {
            var table = _context.Tables
                .Include(t => t.Billings)
                .ThenInclude(b => b.Payment)    
                .Include(t => t.Orders.Where(o => o.OrderStatusId == 4)) // only orders that requested bill
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Prod)
                .FirstOrDefault(t => t.TableId == tableid);

            if (table == null || !table.Orders.Any())
                return Json(new {success = false, message = "Error", detail = "No billable orders found." });

            var disc = _context.DiscountTypes.FirstOrDefault(x => x.DiscountTypeId == discountid);
            Billing billing = new Billing();

            if (disc == null)
            {
                var subtotal = (decimal)table.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.Prod.Price));
                var vatsales = subtotal / 1.12m;
                var tax = subtotal - vatsales;
                var servicecharge = vatsales * 0.10m;
                var grandTotal = vatsales + tax + servicecharge;

                billing = new Billing()
                {
                    BillingId = Guid.NewGuid().ToString(),
                    TableId = tableid,
                    Subtotal = Math.Round(subtotal,2),
                    VatAmount = Math.Round(tax,2),
                    ServiceCharge = Math.Round(servicecharge, 2),
                    GrandTotal = Math.Round(grandTotal, 2),
                    BillingTime = DateTime.Now,
                    Payment = null
                };

                _context.Billings.Add(billing);

                foreach (var order in table.Orders)
                {
                    var bo = new BillingOrder
                    {
                        BillingId = billing.BillingId,
                        OrderId = order.OrderId
                    };
                    _context.Add(bo);
                    order.OrderStatusId = 4; // Billed
                }
            }
            else
            {
                billing = _context.Billings
                    .Include(x => x.Payment)
                    .FirstOrDefault(x => x.Payment == null && x.TableId == tableid);

                var vatsales = Math.Round((decimal)(billing.Subtotal / 1.12m),2);
                var tax = Math.Round((decimal)(billing.Subtotal - vatsales),2);
                var discountedSubtotal = Math.Round((decimal)(vatsales - (vatsales * disc.Percentage)),2);
                var servicecharge = Math.Round((decimal)(discountedSubtotal * 0.10m), 2);
                var grandTotal = Math.Round((decimal)(discountedSubtotal + servicecharge), 2);

                billing.ServiceCharge = Math.Round((decimal)servicecharge, 2);
                billing.GrandTotal = Math.Round((decimal)grandTotal,2);
                billing.BillingTime = DateTime.Now;
                billing.DiscountId = disc.DiscountTypeId;
                billing.Discount = disc;

                _context.Update(billing);
            }


            // Update table status (4 = Waiting for Payment)
            table.TableStatusId = 4;


            _context.SaveChanges();
            var vats = billing.Subtotal / 1.12m;
            decimal? discountAmount = null;

            if (disc != null)
            {
                discountAmount = vats * disc.Percentage;
            }

            return Json(new
            {
                success = true,
                message = "Success",
                discount = disc == null ? null : new
                {
                    disc.DiscountName,
                    disc.Percentage,
                    amount = discountAmount
                },
                bill = new
                {
                    subtotal = billing.Subtotal,
                    vatsales = vats,
                    vatamount = billing.VatAmount,
                    servicecharge = billing.ServiceCharge,
                    total = billing.GrandTotal
                }
            });
        }
        [HttpGet]
        public JsonResult GetBill(string tableid)
        {
            var bill = _context.Billings
               .Include(b => b.Payment)
               .Include(b => b.Discount)
               .FirstOrDefault(b => b.TableId == tableid && b.Payment == null);

            if (bill == null)
                return Json(new { success = false, message = "Bill not found" });

            var result = new
            {
                bill.BillingId,
                Discount = bill.Discount == null ? null : new
                {
                    bill.Discount.DiscountName,
                    bill.Discount.Percentage
                },
                bill.Subtotal,
                bill.VatAmount,
                bill.ServiceCharge,
                bill.GrandTotal,
                Payment = bill.Payment == null ? null : new
                {
                    bill.Payment.AmountPaid,
                    bill.Payment.PaymentTime
                }
            };

            return Json(new { success = true, bill = result });
        }

        [HttpPost]
        public JsonResult PayBill(string tableid, decimal amount, string discount = null)
        {
            var bills = _context.Billings.ToList();
            var bill = _context.Billings
                .Include(b => b.Payment)
                .Include(b => b.Discount)
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

            if (amount < bill.GrandTotal)
                return Json(new { success = false, message = "Insufficient payment" });
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
                    time = payment.PaymentTime,
                    discount = bill.Discount == null ? 0 : bill.Subtotal * bill.Discount.Percentage
                    //discount = new {
                    //    amount = discounted
                    //}
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
