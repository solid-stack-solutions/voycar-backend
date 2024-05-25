namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Post.Registration;

public class EmailService : IEmailService
{
    public void SendEmail(Member member)
    {
        var email = new MimeMessage();
        var smtpEmail = Environment.GetEnvironmentVariable("SmtpEmail");
        var smtpAppPassword = Environment.GetEnvironmentVariable("SmtpAppPassword");

        if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpAppPassword))
        {
            throw new InvalidOperationException("SMTP email or password environment variables are not set.");
        }

        email.From.Add(MailboxAddress.Parse(smtpEmail));
        email.To.Add(MailboxAddress.Parse(member.Email));
        email.Subject = "Konto-Verifizierung";

        var verificationLink = $"http://localhost:8080/api/verify/{member.VerificationToken}"; // not working yet
        var content = $"Bitte klicken Sie auf den folgenden Link, um Ihr Konto zu verifizieren: <a href=\"{verificationLink}\">Link zum Verifizieren</a>";
        var htmlContent = $"<html><body><p style='font-weight: bold;'>{content}</p></body></html>";

        email.Body = new TextPart("html")
        {
            Text = htmlContent
        };

        using var smtpClient = new SmtpClient();
        try
        {
            smtpClient.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
            smtpClient.Authenticate(smtpEmail, smtpAppPassword);
            smtpClient.Send(email);
        }
        finally
        {
            smtpClient.Disconnect(true);
        }
    }
}
