using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.ViewModels;

namespace TheSocialCebu_Capstone.Controllers
{
    public class MenuController : Controller
    {
        //Database
        private readonly MyDBContext _context;

        //Constructor
        public MenuController(MyDBContext context)
        {
            _context = context;
        }

        //List of products
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).Include(p => p.Subcategory).ToList();
            return View(products);
        }

        //Create 
        public IActionResult Create()
        {
            return View(GetViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            //Invalid
            if (!ModelState.IsValid)
            {
                //Repopulate Categories
                vm.Categories = GetCategories();
                vm.Subcategories = GetSubCategories();

                //foreach(var invalid in ModelState)
                return View(vm);
            }

            //Set Product
            var product = new Product
            {
                ProdId = Guid.NewGuid().ToString(),
                ProdName = vm.ProdName,
                Description = vm.Description,
                Price = vm.Price,
                CategoryId = vm.CategoryId,
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
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var vm = new ProductVM
            {
                ProdId = product.ProdId,
                ProdName = product.ProdName,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                SubcategoryId = product.SubcategoryId,
                Availability = product.Availability,
                ExistingImage = product.ProdImage,
                Categories = GetCategories(),
                Subcategories = GetSubCategories()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM vm)
        {
            //Invalid
            if (!ModelState.IsValid)
            {
                //Repopulate
                vm.Categories = GetCategories();
                vm.Subcategories = GetSubCategories();
                return View(vm);
            }
            //Get product
            var product = _context.Products.Find(vm.ProdId);
            if (product == null) return NotFound();

            //Set product's new data
            product.ProdName = vm.ProdName;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.CategoryId = vm.CategoryId;
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

        //
        //Custom Methods
        //

        //Get Categories
        private IEnumerable<SelectListItem> GetCategories() =>
            _context.Categories.Select(c => new SelectListItem { Value = c.CategoryId.ToString(), Text = c.CategoryName }).ToList();

        //Get SubCategories
        private IEnumerable<SelectListItem> GetSubCategories() =>
            _context.SubCategories.Select(s => new SelectListItem { Value = s.SubcategoryId.ToString(), Text = s.SubcategoryName }).ToList();

        //Repopulate VM categories and subcategories
        private ProductVM GetViewModel() => new ProductVM
        {
            Categories = GetCategories(),
            Subcategories = GetSubCategories()
        };
    }
}
