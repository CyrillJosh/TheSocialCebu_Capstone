//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using TheSocialCebu_Capstone.Context;

//namespace Demo.Controllers
//{
//    public class MarketingController : Controller
//    {
//        private readonly MyDBContext _context;
//        private readonly EmailService _emailService;

//        public MarketingController(MyDBContext context, EmailService emailService)
//        {
//            _context = context;
//            _emailService = emailService;

//        }
//        [Authorize(Roles = "Manager")]
//        public IActionResult Subscribe()
//        {
//            return View();
//        }
//        // POST: /MarketingInvite/Subscribe
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Subscribe(EmailInvite model)
//        {
//            if (ModelState.IsValid)
//            {
//                // Avoid duplicates
//                var exists = _context.EmailInvites
//                    .Any(x => x.EmailAddress == model.EmailAddress);

//                if (!exists)
//                {
//                    _context.EmailInvites.Add(model);
//                    _context.SaveChanges();

//                    ViewBag.Message = "Thanks! You’ll receive our invites and offers.";
//                }
//                else
//                {
//                    ViewBag.Message = "This email is already subscribed.";
//                }
//            }

//            return View(model);
//        }

//        // GET: Email/Send
//        [Authorize(Roles = "Manager")]
//        public ActionResult Send()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Send(string subject, string message)
//        {

//            if (!_context.EmailInvites.Any())
//            {
//                ViewBag.Message = "No subscribers found!";
//                return View();
//            }

//            BackgroundJob.Enqueue(() =>
//                     _emailService.SendEmailAsync(subject, message)
//                 );

//            ViewBag.Message = "Emails are being sent in the background!";
//            return View();
//        }

//        // Runs asynchronously in background
//        private async Task SendEmailsInBackground(string subject, string message)
//        {
//            try
//            {
//                using (var scope = HttpContext.RequestServices.CreateScope())
//                {
//                    var dbContext = scope.ServiceProvider.GetRequiredService<MyDBContext>();
//                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

//                    await emailService.SendEmailAsync(subject, message);
//                }
//            }
//            catch (Exception ex)
//            {
//                // Log exception here
//                Console.WriteLine(ex);
//            }
//        }

//        // Show sent emails

//        [Authorize(Roles = "Manager,Front Staff")]

//        public ActionResult SentEmails()
//        {
//            var emails = _context.EmailSents.OrderByDescending(e => e.SentAt).ToList();
//            return View(emails);
//        }
//    }
//}
