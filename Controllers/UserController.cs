using System;
using System.Net.Http.Headers;
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
            if (!ModelState.IsValid)
                return View();

            if (_context == null)
            {
                TempData["ErrorMessage"] = "Database context not initialized.";
                return View();
            }

            var exist = _context.Accounts
                .Include(u => u.Person)
                    .ThenInclude(x => x.Role)
                .FirstOrDefault(a => a.Username == user.Username);

            if (exist == null)
            {
                TempData["ErrorMessage"] =  "Username or Password is incorrect.";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(user.Password, exist.Password))
            {
                TempData["ErrorMessage"] = "Username or Password is incorrect.";
                return View();
            }

            TempData["ErrorMessage"] = "";

            var userRole = exist.Person.Role.RoleName ?? "Unknown";

            if (HttpContext.Session == null)
            {
                TempData["ErrorMessage"] = "Session is not available.";
                return View();
            }

            HttpContext.Session.SetString("_Id", exist.AccountId.ToString());
            HttpContext.Session.SetString("_Role", userRole);

            switch (userRole)
            {
                case "Manager":
                    return RedirectToAction("Index", "Menu");
                case "Kitchen":
                    return RedirectToAction("Index", "Kitchen");
                case "Cashier":
                    return RedirectToAction("Index", "Order");
                default:
                    TempData["ErrorMessage"] = "User role is not recognized.";
                    return View();
            }
        }

        public IActionResult Account(string id)
        {
            var acc = _context.Accounts.Include(x => x.Person).ThenInclude(r => r.Role).FirstOrDefault(x => x.AccountId == id);
            return View(acc);
        }
        [HttpPost]
        public IActionResult UpdateAccount(Account account)
        {
            var existing = _context.Accounts
                .Include(a => a.Person)
                .FirstOrDefault(a => a.AccountId == account.AccountId);

            if (existing != null)
            {
                existing.Username = account.Username;
                existing.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
                existing.DateUpdated = DateTime.Now;

                existing.Person.Name = account.Person.Name;
                existing.Person.BirthDate = account.Person.BirthDate;
                existing.Person.Gender = account.Person.Gender;
                existing.Person.Status = account.Person.Status;

                _context.SaveChanges();
            }

            return RedirectToAction("Account", new { id = account.Person.PersonId });
        }
        public IActionResult HomePage()
        {
            List<Person> people = _context.People.Include(p => p.Account).Include(x => x.Role).ToList();
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

            if (person is null) return Json(new { success = false, message = "Record not found please try again" });

            person.Status = false;
            _context.Update(person);
            _context.SaveChanges();

            return Json(new { success = true, message = "Success!" });
        }

        public JsonResult ResetPassword(string id, string npass)
        {
            var acc = _context.Accounts.FirstOrDefault(x => x.PersonId == id);
            if (acc == null) return Json(new { success = false, message = "User not found" });

            acc.Password = BCrypt.Net.BCrypt.HashPassword(npass);

            _context.Accounts.Update(acc);
            _context.SaveChanges();
            return Json(new {success = true, message="Success"});
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // clears all session data
            return RedirectToAction("Login");
        }
    }
}
