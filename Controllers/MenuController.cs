using Menu.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Diagnostics;
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
        private List<Category> Categories;
        private List<SubCategory> Subcategories;

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

       


        //
        //Custom Methods
        //

        //Populate Categories
        private void PopulateCategories()
        {
            Categories = _context.Categories.ToList();
            Subcategories = _context.SubCategories.Include(x => x.Category).ToList();
        }
    }
}
