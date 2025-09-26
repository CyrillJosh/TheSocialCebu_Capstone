using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Models.BillingClasses;
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
                        }).ToList(),
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
        public JsonResult GenerateBill(string tableid)
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

            Billing billing = new Billing();

            var subtotal = (decimal)table.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.Prod.Price));
            var vatsales = subtotal / 1.12m;
            var tax = subtotal - vatsales;
            var servicecharge = vatsales * 0.10m;
            var grandTotal = vatsales + tax + servicecharge;

            billing = new Billing()
            {
                BillingId = Guid.NewGuid().ToString(),
                TableId = tableid,
                Subtotal = Math.Round(subtotal,3),
                VatAmount = Math.Round(tax,3),
                ServiceCharge = Math.Round(servicecharge, 3),
                GrandTotal = Math.Round(grandTotal, 3),
                BillingTime = DateTime.Now,
                Payment = null,
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

            // Update table status (4 = Waiting for Payment)
            table.TableStatusId = 4;


            _context.SaveChanges();
            var vats = billing.Subtotal / 1.12m;

            return Json(new
            {
                success = true,
                message = "Success",
                bill = new
                {
                    subtotal = Math.Round((decimal)billing.Subtotal,2),
                    vatsales = Math.Round((decimal)vats, 2),
                    vatamount = Math.Round((decimal)billing.VatAmount, 2),
                    servicecharge = Math.Round((decimal)billing.ServiceCharge, 2),
                    total = Math.Round((decimal)billing.GrandTotal, 2)
                }
            });
        }
        [HttpPost]
        public JsonResult ApplyDiscount(string tableid, string discountid, int num, int applic)
        {
            var table = _context.Tables
                  .Include(t => t.Billings)
                  .ThenInclude(b => b.Payment)
                  .Include(t => t.Orders.Where(o => o.OrderStatusId == 4)) // only orders that requested bill
                  .ThenInclude(o => o.OrderItems)
                  .ThenInclude(oi => oi.Prod)
                  .FirstOrDefault(t => t.TableId == tableid);

            if (table == null || !table.Orders.Any())
                return Json(new { success = false, message = "Error: No billable orders found." });

            var disc = _context.DiscountTypes.FirstOrDefault(x => x.DiscountTypeId == discountid);
            if(disc == null)
                return Json(new { success = false, message = "Error: Discount type not found." });


            Billing billing = new Billing();
            //get unpaid bill
            billing = _context.Billings
                   .Include(x => x.Payment)
                   .FirstOrDefault(x => x.Payment == null && x.TableId == tableid);
            //initialization
            var subtotal = billing.Subtotal;
            var numofcust = num;
            var numdischold = applic;

            //calculation
            var vatexempt = (subtotal / numofcust)/1.12m;
            var discountamount = vatexempt * disc.Percentage * numdischold;
            var totaldisc = (vatexempt * numofcust) - discountamount;
            var servicecharge = totaldisc * .10m;
            var vatable = vatexempt * (numofcust - numdischold);
            var vatamount = vatable * .12m;
            var grandtotal = totaldisc + servicecharge + vatamount;
            var vatexemptsale = (totaldisc - vatexempt) + servicecharge;
            var totalsale = totaldisc + servicecharge;

            //update bill
            billing.VatAmount = Math.Round((decimal)vatamount,2);
            billing.ServiceCharge = Math.Round((decimal)servicecharge,2);
            billing.GrandTotal = Math.Round((decimal)grandtotal,2);
            billing.DiscountId = discountid;
            billing.BillingTime = DateTime.Now;

            var DiscountDetails = new DiscountDetail()
            {
                DiscountDetailId = Guid.NewGuid().ToString(),
                BillingId = billing.BillingId,
                DiscountTypeId = disc.DiscountTypeId,
                NumOfCustomer = numofcust,
                NumOfDiscountHolder = numdischold,
            };
            
            _context.DiscountDetails.Add(DiscountDetails);

            //update db
            _context.Update(billing);
            _context.SaveChanges();

            //initialization
            var vats = billing.Subtotal / 1.12m;
            decimal? discountAmount = vats * disc.Percentage;

            return Json(new
            {
                success = true,
                message = "Success",
                discount = disc == null ? null : new
                {
                    disc.DiscountName,
                    disc.Percentage,
                    amount = Math.Round((decimal)discountamount,2),
                    numofcust,
                    numdischold
                },
                bill = new
                {
                    subtotal,
                    vatexempt,
                    servicecharge,
                    vatamount,
                    totalsale
                },
                breakdown = new
                {
                    vatsales = Math.Round((decimal)vatable,2),
                    vatexempt = Math.Round((decimal)vatexemptsale,2),
                    grandtotal = Math.Round((decimal)grandtotal,2)
                }
            });
        }
        [HttpGet]
        public JsonResult GetBill(string tableid)
        {
            var bills = _context.Billings.Include(x=> x.Payment).ToList();
            var bill = _context.Billings
               .Include(b => b.Payment)
               .Include(b => b.Discount)
               .Include(b => b.DiscountDetail)
               .FirstOrDefault(b => b.TableId == tableid && b.Payment == null);

            if (bill == null)
                return Json(new { success = false, message = "Bill not found" });


            //calculation
            var vatexempt = (bill.Subtotal / bill.DiscountDetail.NumOfCustomer) / 1.12m;
            var discountamount = vatexempt * bill.Discount.Percentage * bill.DiscountDetail.NumOfDiscountHolder;
            var totaldisc = (vatexempt * bill.DiscountDetail.NumOfCustomer) - discountamount;
            var servicecharge = totaldisc * .10m;
            var vatable = vatexempt * (bill.DiscountDetail.NumOfCustomer - bill.DiscountDetail.NumOfDiscountHolder);
            var vatamount = vatable * .12m;
            var grandtotal = totaldisc + servicecharge + vatamount;
            var vatexemptsale = (totaldisc - vatexempt) + servicecharge;
            var totalsale = totaldisc + servicecharge;

            var vats = bill.Subtotal / 1.12m;
            decimal? discountAmount = null;

            if (bill.Discount != null)
            {
                discountAmount = Math.Round((decimal)(vats * bill.Discount.Percentage),2);
            }
            return Json(new
            {
                success = true,
                message = "Success",
                discount = bill.DiscountId == null ? null : new
                {
                    bill.Discount.DiscountName,
                    bill.Discount.Percentage,
                    amount = Math.Round((decimal)discountamount, 2),
                    bill.DiscountDetail.NumOfCustomer,
                    bill.DiscountDetail.NumOfDiscountHolder
                },
                bill = new
                {
                    bill.Subtotal,
                    vatexempt,
                    servicecharge,
                    vatamount,
                    totalsale
                },
                breakdown = new
                {
                    vatsales = Math.Round((decimal)vatable, 2),
                    vatexempt = Math.Round((decimal)vatexemptsale, 2),
                    grandtotal = Math.Round((decimal)grandtotal, 2)
                }
            });
        }
        [HttpPost]
        public JsonResult PayBill(string tableid, decimal amount)
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
                .FirstOrDefault(b => b.TableId == tableid && b.Payment == null); 
            if(bill == null)
                return Json(new { success = false, message = "Bill already paid" });
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


            var vats = bill.Subtotal / 1.12m;
            decimal? discountAmount = null;

            if (bill.DiscountId != null)
            {
                discountAmount = vats * bill.Discount.Percentage;
            }
            return Json(new
            {
                success = true,
                vatSales = bill.Subtotal / 1.12m,
                subtotal = bill.Subtotal,
                tax = bill.VatAmount,
                servicecharge = bill.ServiceCharge,
                total = bill.GrandTotal,
                change = amount - bill.GrandTotal,
                discount = bill.Discount == null ? null : new
                {
                    bill.Discount.DiscountName,
                    bill.Discount.Percentage,
                    amount = discountAmount
                },
                payment = new
                {
                    id = payment.PaymentId,
                    amountPaid = payment.AmountPaid,
                    time = payment.PaymentTime,
                    //discount = new {
                    //    amount = discounted
                    //}
                }
            });
        }
    }
}
