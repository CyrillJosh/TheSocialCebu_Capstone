using System.Net;
using System.Net.Mail;
using Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Models;

namespace TheSocialCebu_Capstone.Services
{
    public class EmailService
    {
        private readonly MyDBContext _context;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(MyDBContext context, IOptions<SmtpSettings> smtpSettings)
        {
            _context = context;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string subject, string body)
        {
            var recipients = await _context.Marketings
                .Select(e => e.Email)
                .ToListAsync();

            using (var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                smtp.EnableSsl = true;

                foreach (var email in recipients)
                {
                    // Validate email before sending
                    try
                    {
                        var mailAddress = new MailAddress(email); // will throw if invalid

                        using (var mail = new MailMessage())
                        {
                            mail.From = new MailAddress(_smtpSettings.Username, "TheSocialCebu");
                            //mail.To.Add(mailAddress);
                            mail.Subject = subject;
                            mail.Body = body;
                            mail.IsBodyHtml = false;

                            await smtp.SendMailAsync(mail);
                        }
                    }
                    catch(Exception err)
                    {
                        Console.WriteLine(err);
                        // Skip invalid emails
                        continue;
                    }
                }

                // Save sent email to DB (only once per batch)
                var sentEmail = new EmailInvite
                {
                    Subject = subject,
                    Message = body
                };
                _context.EmailInvites.Add(sentEmail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
