using System;
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
        public IActionResult Login(Account acc)
        {
            //Check for User
            if (_context.Users.Any(x => x.Name == acc.Username) && !ModelState.IsValid)
            {
                //Get person
                var currAcc = _context.Accounts.Where(x => x.Username == acc.Username).FirstOrDefault();
                if (!BCrypt.Net.BCrypt.Verify(acc.PasswordHash, currAcc.PasswordHash))
                {
                    //Invalid
                    return View();
                }
                //Set Session String
                HttpContext.Session.SetString("_Id", currAcc.UserId.ToString());
                HttpContext.Session.SetString("_AccountId", currAcc.AccountId.ToString());
                HttpContext.Session.SetString("_Role", _context.Roles.FirstOrDefault(x => x.RoleId == currAcc.AccountId).ToString());


                return RedirectToAction("HomePage");
            }
            //Invalid
            return View();
        }

        //HomePage
        [Auth("Admin,Manager")]
        public IActionResult HomePage()
        {
            List<Person> people = _context.People.Include(p => p.User).ToList();
            return View(people);
        }
        //Create
        [HttpGet]
        [Auth("Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        //CreateProcess
        [Auth("Admin,Manager")]
        public IActionResult Create(Person person)
        {
            if (!ModelState.IsValid) return View(person);

            person.DateCreated = DateTime.Now;
            person.User.Password = BCrypt.Net.BCrypt.HashPassword(person.User.Password);
            _context.People.Add(person);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Update
        [HttpGet]
        [Auth("Admin,Manager")]
        public IActionResult Update(int id)
        {
            Person person = _context.People.Include(p => p.User).FirstOrDefault(x => x.Id == id);

            return View(person);
        }
        //Update Process
        [Auth("Admin,Manager")]
        public IActionResult Update(Person person)
        {
            //Validate
            if (!ModelState.IsValid)
            {
                return View(person);
            }

            person.DateUpdated = DateTime.Now;

            _context.Update(person);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Delete Process
        [Auth("Admin,Manager")]
        public IActionResult Delete(int id)
        {
            Person person = _context.People.Include(p => p.User).FirstOrDefault(x => x.Id == id);

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
