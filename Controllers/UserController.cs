using System;
using BCrypt.Net;
using Menu.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.ViewModels;

namespace Menu.Controllers
{
    public class UserController : Controller
    {
        //Fields
        private readonly MyDBContext _context;

        //Constructor

        public UserController(MyDBContext context)
        {
            _context = context;
        }

        //Index
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Login
        [HttpPost]
        public IActionResult Login(LoginVM user)
        {
            if(!ModelState.IsValid) return View();

            var exist = _context.Users.Include(u => u.Role).Include(u => u.Person).FirstOrDefault(a => a.Username == user.Username);

            if (!BCrypt.Net.BCrypt.Verify(user.Password, exist.Password))
            {
                //Invalid
                return View();
            }

            //var userRole = _context.Users.FirstOrDefault(u => u == exist).Role.RoleId;

            //Set Session String
            HttpContext.Session.SetString("_Id", exist.AccountId.ToString());
            //HttpContext.Session.SetString("_Role", userRole);

            return RedirectToAction("Index","Menu");
        }

        ////HomePage
        //public IActionResult HomePage()
        //{
        //    return View();
        //}
        ////Create
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        ////CreateProcess
        //public IActionResult Create()
        //{
        //    return RedirectToAction("Index");
        //}
        ////Update
        //[HttpGet]
        //public IActionResult Update()
        //{

        //    return View();
        //}
        ////Update Process
        //[Auth("Admin,Manager")]
        //public IActionResult Update()
        //{

        //    return RedirectToAction("Index");
        //}
        ////Delete Process
        //public IActionResult Delete(int id)
        //{
        //    return Json(new { success = true, message = "Record successfully removed!" });
        //}

        //[HttpPost]
        //public IActionResult Logout()
        //{
        //    HttpContext.Session.Clear(); // clears all session data
        //    return RedirectToAction("Login", "User");
        //}
    }
}
