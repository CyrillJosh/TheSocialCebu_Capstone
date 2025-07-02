//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Runtime.InteropServices;
//using TheSocialCebu_Capstone.Context;
//using TheSocialCebu_Capstone.Models;

//namespace TheSocialCebu_Capstone.Controllers
//{
//    public class EmpMgmtController : Controller
//    {
//        //Fields
//        private readonly MyDBContext _context;

//        //Constructor

//        public EmpMgmtController(MyDBContext context)
//        {
//            _context = context;
//        }

//        //Index
//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        //Login
//        [HttpPost]
//        public IActionResult Login(User user)
//        {
//            //Check for User
//            if (_context.Users.Any(x => x.Username == user.Username) && !ModelState.IsValid)
//            {
//                //Get person
//                var currUser = _context.Users.Where(x => x.Username == user.Username).FirstOrDefault();
//                if (!BCrypt.Net.BCrypt.Verify(user.Password, currUser.Password))
//                {
//                    //Invalid
//                    return View();
//                }
//                //Set Session String
//                HttpContext.Session.SetString("_Id", currUser.Id.ToString());
//                HttpContext.Session.SetString("_Role", currUser.Role.ToString());

//                return RedirectToAction("HomePage");
//            }
//            //Invalid
//            return View();
//        }

//        //HomePage
//        [Auth("Admin,Manager")]
//        public IActionResult HomePage()
//        {
//            List<Person> people = _context.People.Include(p => p.User).ToList();
//            return View(people);
//        }
//        //Create
//        [HttpGet]
//        [Auth("Admin,Manager")]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        //CreateProcess
//        [Auth("Admin,Manager")]
//        public IActionResult Create(Person person)
//        {
//            if (!ModelState.IsValid) return View(person);

//            person.DateCreated = DateTime.Now;
//            person.User.Password = BCrypt.Net.BCrypt.HashPassword(person.User.Password);
//            _context.People.Add(person);
//            _context.SaveChanges();

//            return RedirectToAction("Index");
//        }
//        //Update
//        [HttpGet]
//        [Auth("Admin,Manager")]
//        public IActionResult Update(int id)
//        {
//            Person person = _context.People.Include(p => p.User).FirstOrDefault(x => x.Id == id);

//            return View(person);
//        }
//        //Update Process
//        [Auth("Admin,Manager")]
//        public IActionResult Update(Person person)
//        {
//            //Validate
//            if (!ModelState.IsValid)
//            {
//                return View(person);
//            }

//            person.DateUpdated = DateTime.Now;

//            _context.Update(person);
//            _context.SaveChanges();

//            return RedirectToAction("Index");
//        }
//        //Delete Process
//        [Auth("Admin,Manager")]
//        public IActionResult Delete(int id)
//        {
//            Person person = _context.People.Include(p => p.User).FirstOrDefault(x => x.Id == id);

//            if (person is null) return Json(new { success = false, message = "Error! Record not found please try again" });

//            _context.Remove(person);
//            _context.SaveChanges();

//            return Json(new { success = true, message = "Record successfully removed!" });
//        }

//        [HttpPost]
//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear(); // clears all session data
//            return RedirectToAction("Login", "User");
//        }
//    }
//}
