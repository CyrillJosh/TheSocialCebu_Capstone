using Microsoft.AspNetCore.Mvc;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.Controllers
{
    public class MarketingController : Controller
    {
        private readonly MyDBContext _context;
        public MarketingController(MyDBContext context)
        {
            _context = context;
        }

        public JsonResult Rate(int rate, string email)
        {
            try
            {
                if (rate == 0 )
                    return Json(new { success = false });

                var feedback = new Feedback() { 
                    FeedbackId = Guid.NewGuid().ToString(),
                    Rating = rate,
                    DateCreated = DateTime.Now
                };
                _context.Feedbacks.Add(feedback);

                if(!string.IsNullOrEmpty(email))
                {
                    var mark = new Marketing()
                    {
                        EmailId = Guid.NewGuid().ToString(),
                        Email = email,
                    };
                    _context.Marketings.Add(mark);
                }

                _context.SaveChanges();


                return Json(new { success = true });
            }catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex}" });
            }
        }
    }
}
