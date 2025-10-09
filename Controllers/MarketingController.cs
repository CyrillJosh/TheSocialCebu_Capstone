using Menu.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;
using TheSocialCebu_Capstone.Services;

namespace TheSocialCebu_Capstone.Controllers
{
    [Auth("Manager")]
    public class MarketingController : Controller
    {

        private readonly MyDBContext _context;
        private readonly EmailService _emailservice;
        public MarketingController(MyDBContext context, EmailService emailService)
        {
            _context = context;
            _emailservice = emailService;

        }

        public IActionResult Index()
        {
            var f = _context.Feedbacks.ToList();
            return View(f);
        }


        public JsonResult Rate(int rate, string email)
        {
            try
            {
                // Validate rating
                if (rate == 0)
                    return Json(new { success = false, message = "Rating cannot be 0." });

                // Validate email if provided
                if (!string.IsNullOrEmpty(email))
                {
                    try
                    {
                        var mail = new MailAddress(email); // Throws if invalid
                    }
                    catch
                    {
                        return Json(new { success = false, message = "Invalid email address." });
                    }
                }

                // Add Marketing entry if email not already exists
                if (!string.IsNullOrEmpty(email) && !_context.Marketings.Any(x => x.Email == email))
                {
                    var market = new Marketing()
                    {
                        EmailId = Guid.NewGuid().ToString(),
                        Email = email,
                    };
                    _context.Marketings.Add(market);
                }

                // Add feedback
                var feedback = new Feedback()
                {
                    FeedbackId = Guid.NewGuid().ToString(),
                    Rating = rate,
                    DateCreated = DateTime.Now
                };
                _context.Feedbacks.Add(feedback);

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
