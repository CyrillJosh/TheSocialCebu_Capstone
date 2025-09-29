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

        public IActionResult HomePage()
        {
            List<Person> people = _context.People.Include(p => p.Account).ToList();
            return View(people);
        }

        //CreateProcess
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new PersonVM
            {
                Roles = _context.Roles.ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [Auth("Manager")]
        public IActionResult Create(PersonVM personvm)
        {
            var person = new Person()
            {
                PersonId = Guid.NewGuid().ToString(),
                Status = true,
                BirthDate = personvm.BirthDate,
                Gender = personvm.Gender,
                RoleId = personvm.RoleId,
                Name = personvm.Name,
                HiredDate = DateTime.Now,
                Account = new Account()
                {
                    Username = personvm.Username
                }

            };
            personvm.Roles = _context.Roles.ToList();
            if (!ModelState.IsValid) return View(personvm);
            person.Account.Password = BCrypt.Net.BCrypt.HashPassword(personvm.Password);
            person.Account.AccountId = Guid.NewGuid().ToString();
            person.Account.PersonId = person.PersonId;
            person.Role = _context.Roles.FirstOrDefault(x => x.RoleId == person.RoleId);
            person.Account.DateUpdated = DateTime.Now;
            _context.People.Add(person);
            _context.SaveChanges();
            return RedirectToAction("HomePage");
        }
        //Update
        [HttpGet]
        [Auth("Manager")]
        public IActionResult Update(string id)
        {
            var person = _context.People
                .Include(p => p.Account)
                .FirstOrDefault(x => x.PersonId == id);

            if (person == null)
                return NotFound();

            var personvm = new PersonUpdateVM
            {
                PersonId = person.PersonId,
                BirthDate = person.BirthDate,
                HiredDate = person.HiredDate,
                Gender = person.Gender,
                Name = person.Name,
                Status = person.Status,
                RoleId = person.RoleId,
                Roles = _context.Roles.ToList(),
            };

            return View(personvm);
        }

        //Update Process
        [Auth("Manager")]
        public IActionResult Update(PersonUpdateVM personVM)
        {
            if (!ModelState.IsValid)
                return View(personVM);

            var person = _context.People
                                 .Include(p => p.Account)
                                 .FirstOrDefault(p => p.PersonId == personVM.PersonId);

            if (person == null)
                return NotFound();

            // Update only person fields, not password
            person.Name = personVM.Name;
            person.Gender = personVM.Gender;
            person.BirthDate = personVM.BirthDate ?? person.BirthDate;
            person.HiredDate = personVM.HiredDate ?? person.HiredDate; // keep old value if null
            person.Status = personVM.Status;
            person.RoleId = personVM.RoleId;

            // Update account metadata (optional)
            person.Account.DateUpdated = DateTime.Now;

            _context.Update(person);
            _context.SaveChanges();

            return RedirectToAction("HomePage");
        }



        //Delete Process
        [Auth("Manager")]
        public IActionResult Delete(string id)
        {
            Person person = _context.People.Include(p => p.Account).FirstOrDefault(x => x.PersonId == id);

            if (person is null) return Json(new { success = false, message = "Error! Record not found please try again" });

            person.Status = false;
            _context.Update(person);
            _context.SaveChanges();

            return Json(new { success = true, message = "Success!" });
        }

        public JsonResult ResetPassword(string id, string npass)
        {
            var acc = _context.Accounts.FirstOrDefault(x => x.PersonId == id);
            if (acc == null) return Json(new { success = false, message = "Error: User not found" });

            acc.Password = npass;

            _context.Accounts.Update(acc);
            _context.SaveChanges();
            return Json(new {success = true, message="Success"});
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // clears all session data
            return RedirectToAction("Login", "User");
        }
    }
}
