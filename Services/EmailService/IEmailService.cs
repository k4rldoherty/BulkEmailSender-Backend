using BulkEmailSender.Models;

namespace BulkEmailSender.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDTO request);
    }
}
