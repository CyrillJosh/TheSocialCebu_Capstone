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

            var exist = _context.Accounts.Include(u => u.Person).ThenInclude(x => x.Role).FirstOrDefault(a => a.Username == user.Username);

            if (!BCrypt.Net.BCrypt.Verify(user.Password, exist.Password))
            {
                //Invalid
                return View();
            }

            var userRole = exist.Person.Role.RoleId;

            //Set Session String
            HttpContext.Session.SetString("_Id", exist.AccountId.ToString());
            HttpContext.Session.SetString("_Role", userRole);

            return RedirectToAction("Index","Menu");
        }
        //CreateProcess
        [Auth("Manager")]
        public IActionResult Create(Person person)
        {
            if (!ModelState.IsValid) return View(person);

            person.HiredDate = DateTime.Now;
            person.Account.Password = BCrypt.Net.BCrypt.HashPassword(person.Account.Password);
            _context.People.Add(person);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Update
        [HttpGet]
        [Auth("Manager")]
        public IActionResult Update(string id)
        {
            Person person = _context.People.Include(p => p.Account).FirstOrDefault(x => x.PersonId == id);

            return View(person);
        }
        //Update Process
        [Auth("Manager")]
        public IActionResult Update(Person person)
        {
            //Validate
            if (!ModelState.IsValid)
            {
                return View(person);
            }

            person.Account.DateUpdated = DateTime.Now;

            _context.Update(person);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Delete Process
        [Auth("Manager")]
        public IActionResult Delete(string id)
        {
            Person person = _context.People.Include(p => p.Account).FirstOrDefault(x => x.PersonId == id);

            if (person is null) return Json(new { success = false, message = "Error! Record not found please try again" });

            _context.Remove(person);
            _context.SaveChanges();

            return Json(new { success = true, message = "Record successfully removed!" });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // clears all session data
            return RedirectToAction("Login", "User");
        }
    }
}
