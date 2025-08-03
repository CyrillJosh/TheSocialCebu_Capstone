using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Reflection.Emit;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Models.MenuClasses;
using TheSocialCebu_Capstone.ViewModels;

namespace TheSocialCebu_Capstone.Controllers
{
    public class MenuController : Controller
    {
        //Database
        private readonly MyDBContext _context;
        private IEnumerable<SelectListItem> Categories;
        private IEnumerable<SelectListItem> Subcategories;

        //Constructor
        public MenuController(MyDBContext context)
        {
            _context = context;
        }

        //List of products
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Subcategory).ThenInclude(s => s.Category).OrderByDescending(o => o.Availability).ThenBy(o => o.ProdName).ToList();
            return View(products);
        }

        //Create 
        public IActionResult Create()
        {
            PopulateCategories(); 
            //populate VM categories and subcategories
            ProductVM vm = new ProductVM()
            {
                Categories = Categories,
                Subcategories = Subcategories
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            //Invalid
            if (!ModelState.IsValid)
            {
                PopulateCategories();
                //Repopulate Categories
                vm.Categories = Categories;
                vm.Subcategories = Subcategories;

                return View(vm);
            }

            //Set Product
            var product = new Product
            {
                ProdId = Guid.NewGuid().ToString(),
                ProdName = vm.ProdName,
                Description = vm.Description,
                Price = vm.Price,
                SubcategoryId = vm.SubcategoryId,
                Availability = vm.Availability
            };

            //Check for Image
            if (vm.UploadImage != null)
            {
                using var ms = new MemoryStream();
                await vm.UploadImage.CopyToAsync(ms);
                product.ProdImage = ms.ToArray();
            }

            //Add the new product
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            //Return to Index
            return RedirectToAction("Index");
        }

        //Edit
        public IActionResult Edit(string id)
        {
            //Get the product
            var product = _context.Products.Include(s => s.Subcategory).FirstOrDefault(p => p.ProdId == id);
            if (product == null) return NotFound();
            PopulateCategories();

            var vm = new ProductVM
            {
                ProdId = product.ProdId,
                ProdName = product.ProdName,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.Subcategory.CategoryId,
                SubcategoryId = product.SubcategoryId,
                Availability = product.Availability,
                ExistingImage = product.ProdImage,
                Categories = Categories,
                Subcategories = Subcategories
            };                  

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategories();
                //Repopulate Categories
                vm.Categories = Categories;
                vm.Subcategories = Subcategories;

                return View(vm);
            }
            //Get product
            var product = _context.Products.Find(vm.ProdId);
            if (product == null) return NotFound();

            //Set product's new data
            product.ProdName = vm.ProdName;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.SubcategoryId = vm.SubcategoryId;
            product.Availability = vm.Availability;

            //image
            if (vm.UploadImage != null)
            {
                using var ms = new MemoryStream();
                await vm.UploadImage.CopyToAsync(ms);
                product.ProdImage = ms.ToArray();
            }

            //Update product
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            //Return to Index
            return RedirectToAction("Index");
        }

        //Delete
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            //Get product
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            //Delete product (Make product unavailable)
            product.Availability = false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            //Return to Index
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetSubcategories(string categoryId)
        {
            var subcategories = _context.SubCategories
                .Where(sc => sc.CategoryId == categoryId)
                .Select(sc => new { sc.SubcategoryId, sc.SubcategoryName })
                .ToList();

            return Json(subcategories);
        }

        //Digital Menu
        public IActionResult Menu(string id = null, bool reset = false)
        {
            // Clear table if reset is triggered
            if (reset)
            {
                HttpContext.Session.Remove("Table");
                HttpContext.Session.Remove("Order");
                HttpContext.Session.Remove("SuppressModalOnce");
            }

            // Table just selected
            if (!string.IsNullOrEmpty(id))
            {
                var previousTable = HttpContext.Session.GetString("Table");

                // If user selects a DIFFERENT table, clear the cart
                if (previousTable != null && previousTable != id)
                {
                    HttpContext.Session.Remove("Orders");
                }
                HttpContext.Session.SetString("Table", id);
                HttpContext.Session.SetString("SuppressModalOnce", "true");

                var existingOrder = _context.OrderItems
                    .Include(o => o.Order)
                    .FirstOrDefault(x => x.Order.TableId == id);

                HttpContext.Session.SetString("Order", existingOrder != null
                    ? existingOrder.OrderId
                    : Guid.NewGuid().ToString());

                return RedirectToAction("Menu"); // avoid resubmission
            }

            // Read session data
            var sessionTable = HttpContext.Session.GetString("Table");
            var suppressModal = HttpContext.Session.GetString("SuppressModalOnce");

            // Show modal if:
            // (1) Table is not selected yet
            // (2) Or table is selected but user refreshed the page (suppress flag expired)
            ViewBag.ShowTableModal = string.IsNullOrEmpty(suppressModal);
            ViewBag.CurrentTable = sessionTable;
            ViewBag.ActiveTables = _context.Tables.Where(t => t.Status == true).ToList();

            // Remove suppress flag so it only works once
            HttpContext.Session.Remove("SuppressModalOnce");

            // Load products (same as before)
            var products = _context.Products
                .Where(x => x.Availability == true)
                .Include(s => s.Subcategory)
                .ThenInclude(c => c.Category)
                .ToList();

            PopulateCategories();

            var vm = products.Select(p => new ProductVM
            {
                ProdId = p.ProdId,
                ProdName = p.ProdName,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.Subcategory.Category.CategoryId,
                SubcategoryId = p.SubcategoryId,
                Availability = p.Availability,
                ExistingImage = p.ProdImage,
                Categories = Categories,
                Subcategories = Subcategories
            }).ToList();

            return View(vm);
        }

        //Preview Product
        public IActionResult Preview(string id)
        {
            var product = _context.Products.Where(x => x.Availability == true).FirstOrDefault(x => x.ProdId == id);
            if (product == null)
                return NotFound();
            return Json(new
            {
                prodId = product.ProdId,
                prodName = product.ProdName,
                price = product.Price,
                prodImage = Convert.ToBase64String(product.ProdImage ?? new byte[0])
            });
        }

        //
        //Custom Methods
        //

        //Populate Categories
        private void PopulateCategories()
        {
            Categories = _context.Categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName }).ToList();
            Subcategories = _context.SubCategories.Select(s => new SelectListItem { Value = s.SubcategoryId.ToString(), Text = s.SubcategoryName }).ToList();
        }
    }
}
