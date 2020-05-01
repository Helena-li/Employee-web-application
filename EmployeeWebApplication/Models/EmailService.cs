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
        private static string SendGridApiKey = "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";
        private static string TryTemplate = "d-8ea7633ca68c4b16832578595d039743";

        public async void SendEmail(TemplateEmailData templateEmailData)
        {
            try
            {
                var client = new SendGridClient(SendGridApiKey);
                var msg = new SendGridMessage();
                msg.SetFrom(new EmailAddress("lihuiling987@gmail.com", "Linda"));
                msg.AddTo(new EmailAddress("lihuiling987@gmail.com", "Linda"));
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
