namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using System.Net;
using System.Net.Mail;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp-mail.outlook.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SmtpEmail"), Environment.GetEnvironmentVariable("SmtpAppPassword"))
        };

        return client.SendMailAsync(
            new MailMessage(from: Environment.GetEnvironmentVariable("SmtpEmail")!,
                to: email,
                subject,
                message
            ));
    }
}
