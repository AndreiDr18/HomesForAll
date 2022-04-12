using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace HomesForAll.Utils.Mail
{
    public class MailManager
    {
        public static async void SendRegistrationMail(Guid userId, string toMail, string toName)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRIDAPIKEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("dragneaandrei2001@gmail.com", "Dragnea Ionut Andrei");
            var subject = "Email confirmation";
            var to = new EmailAddress(toMail, toName);
            var plainTextContext = $"Verify your email address here: https://localhost:7165/api/auth/verifyEmail/{userId.ToString()}";
            var htmlContent = $"<h1>Verify your email address here: https://localhost:7165/api/auth/verifyEmail/{userId.ToString()}</h1>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContext, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
