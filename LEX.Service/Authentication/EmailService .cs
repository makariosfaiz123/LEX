using LEX.Service_Abstraction.IAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.Authentication
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("makariosfaiz2121@gmail.com", "hzgmhtbrlysrexbg"),
                EnableSsl = true
            };
            
            var message = new MailMessage("YOUR_EMAIL@gmail.com", to, subject, body);

            await smtp.SendMailAsync(message);
        }
    }
}
