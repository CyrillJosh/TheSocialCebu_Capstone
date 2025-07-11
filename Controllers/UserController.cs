using System;
using BCrypt.Net;
using Menu.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;

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
        public IActionResult Login(Account account)
        {
            if(!ModelState.IsValid) return View();

            var exist = _context.Accounts.FirstOrDefault(a => a.Username == account.Username);

            if (!BCrypt.Net.BCrypt.Verify(account.PasswordHash, exist.PasswordHash))
            {
                //Invalid
                return View();
            }

            var userRole = _context.Users.FirstOrDefault(u => u.Account == exist).Role.RoleId;

            //Set Session String
            HttpContext.Session.SetString("_Id", exist.AccountId.ToString());
            HttpContext.Session.SetString("_Role", userRole);

            return RedirectToAction("Menu","Index");
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
