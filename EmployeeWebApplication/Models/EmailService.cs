using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Models
{
    public class EmailService : IEmailService
    {
        private static string SendGridApiKey = "My send grid key";
        private static string TryTemplate = "my template id";

        public async void SendEmail(TemplateEmailData templateEmailData)
        {
            try
            {
                var client = new SendGridClient(SendGridApiKey);
                var msg = new SendGridMessage();
                msg.SetFrom(new EmailAddress("test@gmail.com", "L"));
                msg.AddTo(new EmailAddress("test@gmail.com", "new"));
                msg.SetTemplateId(TryTemplate);

                msg.SetTemplateData(templateEmailData);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
