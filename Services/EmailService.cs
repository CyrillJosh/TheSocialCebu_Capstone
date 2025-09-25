//using System.Net;
//using System.Net.Mail;
//using Configurations;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using TheSocialCebu_Capstone.Context;

//namespace Demo.Services
//{
//    public class EmailService
//    {
//        private readonly MyDBContext _context;
//        private readonly SmtpSettings _smtpSettings;

//        public EmailService(MyDBContext context, IOptions<SmtpSettings> smtpSettings)
//        {
//            _context = context;
//            _smtpSettings = smtpSettings.Value;
//        }

//        public async Task SendEmailAsync(string subject, string body)
//        {
//            var recipients = await _context.EmailInvites
//               .Select(e => e.EmailAddress)
//               .ToListAsync();

//            using (var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
//            {
//                smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
//                smtp.EnableSsl = true;

//                foreach (var email in recipients) 
//                {
//                    var mail = new MailMessage
//                    {
//                        From = new MailAddress(_smtpSettings.Username, "TheSocialNi&&ers"),
//                        Subject = subject,
//                        Body = body,
//                        IsBodyHtml = false
//                    };
//                    mail.To.Add(email);
//                    await smtp.SendMailAsync(mail);
//                }

//                // Save sent email to DB
//                var sentEmail = new EmailSent
//                {
//                    Subject = subject,
//                    Message = body,
//                    SentAt = DateTime.Now
//                };
//                _context.EmailSents.Add(sentEmail);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}