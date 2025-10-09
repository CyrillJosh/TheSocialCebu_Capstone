using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.ViewModels;
using QRCoder;
using Menu.Attributes;

namespace TheSocialCebu_Capstone.Controllers
{
    [Auth("Manager")] 
    public class TableController : Controller
    {
        private readonly MyDBContext _context;

        public TableController(MyDBContext context)
        {
            _context = context;
        }

        //Index
        public IActionResult Index()
        {
            var tables = _context.Tables.Include(t => t.Location).Include(t => t.TableStatus).ToList();
            return View(tables);
        }

        //Create
        public IActionResult Create()
        {
            var vm = new TableVM
            {
                StatusList = _context.TableStatuses.ToList(),
                LocationList = _context.Locations.Select(l => new SelectListItem { Value = l.LocationId.ToString(), Text = l.LocationName }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TableVM vm)
        {
            if (ModelState.IsValid)
            {
                // Correctly get the TableStatusId by matching the string Status name
                var tableStatus = _context.TableStatuses.FirstOrDefault(s => s.TableStatusId == vm.StatusId);

                if (tableStatus == null)
                {
                    ModelState.AddModelError("Status", "Invalid status selected.");
                    return View(vm);
                }

                var newTable = new Table
                {
                    TableId = vm.Id, // Use the ID passed from the view
                    TableNumber = vm.TableNumber.ToUpper(),
                    TableStatusId = tableStatus.TableStatusId, // Correctly map the status string to the integer ID
                    LocationId = vm.LocationId,
                    QrcodeImage = Convert.FromBase64String(vm.QRCodeBase64.Split(',')[1]),
                };

                _context.Add(newTable);
                await _context.SaveChangesAsync();
                return RedirectToAction("Print", new { id = newTable.TableId });
            }
            // Re-populate LocationList for the view in case of a model validation error
            vm.LocationList = _context.Locations.Select(l => new SelectListItem { Value = l.LocationId.ToString(), Text = l.LocationName }).ToList();
            vm.StatusList = _context.TableStatuses.ToList();
            return View(vm);
        }

        //Print
        public IActionResult Print(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Error", "Home", "Error|Table not found!");

            var table = _context.Tables
                .Include(t => t.Location)
                .FirstOrDefault(t => t.TableId == id);

            if (table == null) return RedirectToAction("Error", "Home", "Error|Table not found!");

            return View(table);
        }

        //Edit
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return RedirectToAction("Error", "Home", "Error|Table not found!");

            var table = _context.Tables.Include(x => x.TableStatus).FirstOrDefault(x => x.TableId == id);
            if (table == null) return RedirectToAction("Error", "Home", "Error|Table not found!");

            var vm = new TableVM
            {
                Id = table.TableId,
                TableNumber = table.TableNumber,
                LocationId = table.LocationId,
                StatusId = table.TableStatusId,
                ExistingQRCodeImage = table.QrcodeImage,
                LocationList = _context.Locations.Select(l => new SelectListItem { Value = l.LocationId.ToString(), Text = l.LocationName }).ToList(),
                StatusList = _context.TableStatuses.ToList()
            };


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, TableVM vm)
        {
            if (id.ToString() != vm.Id) return RedirectToAction("Error", "Home", "Error|Table not found!");

            if (ModelState.IsValid)
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null) return RedirectToAction("Error", "Home", "Error|Table not found!");

                table.TableNumber = vm.TableNumber;
                table.LocationId = vm.LocationId;
                table.TableStatusId = vm.StatusId;


                if (!string.IsNullOrEmpty(vm.QRCodeBase64))
                {
                    var base64Data = vm.QRCodeBase64.Split(',')[1];
                    table.QrcodeImage = Convert.FromBase64String(base64Data);
                }

                _context.Update(table);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            vm.StatusList = _context.TableStatuses.ToList(); 
            vm.LocationList = _context.Locations.Select(c => new SelectListItem { Value = c.LocationId.ToString(), Text = c.LocationName }).ToList(); 
            return View(vm);
        }

        //Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //Get table
            var table = await _context.Tables.FindAsync(id);
            //Make table unavailable
            table.TableStatusId = 5;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        //Generate QRCode
        public JsonResult GenerateQRCode(string value)
        {
            var qrcode = new QRCodeGenerator();
            //var qr = qrcode.CreateQrCode("http://192.168.254.177/Order/Table/" + value, QRCodeGenerator.ECCLevel.M); // CY home wifi
            //var qr = qrcode.CreateQrCode("http://thesocial.cebu/Order/Table/" + value, QRCodeGenerator.ECCLevel.M); //testing domain
            var qr = qrcode.CreateQrCode("http://10.41.43.47/Order/Table/" + value, QRCodeGenerator.ECCLevel.M); // CY hotspot


            Base64QRCode qrimage = new Base64QRCode(qr);
            string qrstring = "data:image/png;base64," + qrimage.GetGraphic(20);
            return Json(new { qrstring = qrstring } );
        }
    }
}
