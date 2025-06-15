using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.ViewModels;

namespace TheSocialCebu_Capstone.Controllers
{
    public class TableController : Controller
    {
        private readonly MyDBContext _context;

        public TableController(MyDBContext context)
        {
            _context = context;
        }

        //Index
        public async Task<IActionResult> Index()
        {
            var tables = await _context.Tables.Include(t => t.Location).ToListAsync();
            return View(tables);
        }

        //Create
        public IActionResult Create()
        {
            var vm = new TableVM
            {
                LocationList = _context.Locations.Select(l => new SelectListItem{Value = l.LocationId, Text = l.LocationName}).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TableVM vm)
        {
            if (ModelState.IsValid)
            {
                var newTable = new Table
                {
                    Id = Guid.NewGuid().ToString(),
                    TableNumber = vm.TableNumber,
                    Status = vm.Status,
                    LocationId = vm.LocationId,
                    QrcodeImage = Convert.FromBase64String(vm.QRCodeBase64.Split(',')[1]),
                };

                _context.Add(newTable);
                await _context.SaveChangesAsync();
                return RedirectToAction("Print", new { id = newTable.Id });
            }

            vm.LocationList = new SelectList(_context.Locations, "Location_Id", "Location_Name");
            return View(vm);
        }

        //Print
        public IActionResult Print(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var table = _context.Tables
                .Include(t => t.Location)
                .FirstOrDefault(t => t.Id == id);

            if (table == null) return NotFound();

            return View(table);
        }

        //Edit
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var table = await _context.Tables.FindAsync(id);
            if (table == null) return NotFound();

            var vm = new TableVM
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                LocationId = table.LocationId,
                Status = table.Status,
                ExistingQRCodeImage = table.QrcodeImage,
                LocationList = _context.Locations.Select(l => new SelectListItem { Value = l.LocationId, Text = l.LocationName }).ToList()
            };


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, TableVM vm)
        {
            if (id.ToString() != vm.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var table = await _context.Tables.FindAsync(id);
                if (table == null) return NotFound();

                table.TableNumber = vm.TableNumber;
                table.LocationId = vm.LocationId;
                table.Status = vm.Status;


                if (!string.IsNullOrEmpty(vm.QRCodeBase64))
                {
                    var base64Data = vm.QRCodeBase64.Split(',')[1];
                    table.QrcodeImage = Convert.FromBase64String(base64Data);
                }

                _context.Update(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.LocationList = new SelectList(_context.Locations.Select(c => new SelectListItem { Value = c.LocationId.ToString(), Text = c.LocationName }).ToList()); 
            return View(vm);
        }

        //Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //Get table
            var table = await _context.Tables.FindAsync(id);
            //Make table unavailable
            table.Status = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
