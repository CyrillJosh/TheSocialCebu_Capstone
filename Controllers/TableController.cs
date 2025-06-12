using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                LocationList = new SelectList(_context.Locations.ToList(), "Location_Id", "Location_Name")
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TableVM vm)
        {
            if (ModelState.IsValid)
            {
                var table = new Table
                {
                    Id = Guid.NewGuid().ToString(),
                    TableNumber = vm.TableNumber,
                    LocationId = vm.LocationId
                };

                if (vm.QRCodeImageFile != null && vm.QRCodeImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await vm.QRCodeImageFile.CopyToAsync(ms);
                        table.QrcodeImage = ms.ToArray();
                    }
                }

                _context.Tables.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.LocationList = new SelectList(_context.Locations.ToList(), "Location_Id", "Location_Name", vm.LocationId);
            return View(vm);
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
                LocationList = new SelectList(_context.Locations.ToList(), "Location_Id", "Location_Name", table.LocationId)
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

                if (vm.QRCodeImageFile != null && vm.QRCodeImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await vm.QRCodeImageFile.CopyToAsync(ms);
                        table.QrcodeImage = ms.ToArray();
                    }
                }

                _context.Update(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.LocationList = new SelectList(_context.Locations.ToList(), "Location_Id", "Location_Name", vm.LocationId);
            return View(vm);
        }

        //Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var table = await _context.Tables.FindAsync(id);
            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
