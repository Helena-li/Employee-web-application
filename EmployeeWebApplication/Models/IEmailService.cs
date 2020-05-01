namespace EmployeeWebApplication.Models
{
    public interface IEmailService
    {
        void SendEmail(TemplateEmailData templateEmailData);
    }
}
