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
        //Index
        public IActionResult Index()
        {
            // Only tables that requested a bill or in payment
            var tables = _context.Tables
                .Where(x => x.TableStatusId == 3 || x.TableStatusId == 4)
                .ToList();

            // Get order items
            var orderitems = _context.OrderItems
                .Include(x => x.Order)
                .ThenInclude(x => x.Table)
                .Include(x => x.Prod)
                .ToList();

            //get orders
            var orders = _context.Orders
                .Include(x => x.Table)
                .Where(x => x.OrderStatusId < 5)
                .ToList();

            //ViewModel for display
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
        public IActionResult CompletedBills()
        {
            var bills = _context.Billings
                .Include(x => x.BillingOrders)
                    .ThenInclude(x => x.Order)
                    .ThenInclude(x => x.OrderItems)
                    .ThenInclude(x => x.Prod)
                .Include(x => x.Discount)
                .Include(x => x.DiscountDetail)
                .Include(x => x.Table)
                .Include(x => x.Payment).Where(x => x.Payment != null).ToList();
            return View(bills);
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
                    billing.BillingId,
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
                    .Include(x => x.DiscountDetail)
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

            if (string.IsNullOrEmpty(billing.BillingId))
            {
                _context.Billings.Update(billing);
                _context.SaveChanges();
            }

            var existingDiscount = _context.DiscountDetails
                .FirstOrDefault(x => x.BillingId == billing.BillingId);

            if (existingDiscount != null)
            {
                existingDiscount.DiscountTypeId = disc.DiscountTypeId;
                existingDiscount.NumOfCustomer = numofcust;
                existingDiscount.NumOfDiscountHolder = numdischold;
                _context.DiscountDetails.Update(existingDiscount);
            }
            else
            {
                var newDetail = new DiscountDetail
                {
                    DiscountDetailId = Guid.NewGuid().ToString(),
                    BillingId = billing.BillingId,
                    DiscountTypeId = disc.DiscountTypeId,
                    NumOfCustomer = numofcust,
                    NumOfDiscountHolder = numdischold,
                };
                _context.DiscountDetails.Add(newDetail);
            }


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
                    billing.BillingId,
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
            try
            {
                var bill = _context.Billings
                   .Include(b => b.Payment)
                   .Include(b => b.Discount)
                   .Include(b => b.DiscountDetail)
                   .FirstOrDefault(b => b.TableId == tableid && b.Payment == null);

                if (bill == null)
                    return Json(new { success = false, message = "Bill not found" });

                decimal vatexempt = 0;
                decimal discountamount = 0;
                decimal totaldisc = 0;
                decimal servicecharge = 0;
                decimal vatable = 0;
                decimal vatamount = 0;
                decimal grandtotal = 0;
                decimal vatexemptsale = 0;
                decimal totalsale = 0;

                if (bill.Discount != null && bill.DiscountDetail != null)
                {
                    vatexempt = (decimal)((bill.Subtotal / bill.DiscountDetail.NumOfCustomer) / 1.12m);
                    discountamount = (decimal)(vatexempt * bill.Discount.Percentage * bill.DiscountDetail.NumOfDiscountHolder);
                    totaldisc = (decimal)((vatexempt * bill.DiscountDetail.NumOfCustomer) - discountamount);
                    servicecharge = totaldisc * .10m;
                    vatable = (decimal)(vatexempt * (bill.DiscountDetail.NumOfCustomer - bill.DiscountDetail.NumOfDiscountHolder));
                    vatamount = vatable * .12m;
                    grandtotal = totaldisc + servicecharge + vatamount;
                    vatexemptsale = (totaldisc - vatexempt) + servicecharge;
                    totalsale = totaldisc + servicecharge;
                }
                else
                {
                    // No discount applied
                    vatexempt = (decimal)(bill.Subtotal / 1.12m);
                    vatamount = (decimal)(bill.Subtotal - vatexempt);
                    servicecharge = vatexempt * 0.10m;
                    grandtotal = (decimal)(bill.Subtotal + servicecharge);
                    totalsale = (decimal)(bill.Subtotal + servicecharge);
                    vatable = vatexempt;
                    vatexemptsale = vatexempt + servicecharge;
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
                        bill.BillingId,
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                return Json(new { success = false, message = "Bill not found" });

            if (bill.Payment != null)
                return Json(new { success = false, message = "Bill already paid" });

            if (amount < bill.GrandTotal)
                return Json(new { success = false, message = "Insufficient payment amount" });
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
                bill.BillingId,
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
