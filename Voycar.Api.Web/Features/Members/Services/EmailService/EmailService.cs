namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

/// <summary>
/// Service for sending verification emails to members.
///
/// Uses SMTP settings from environment variables to send emails via Gmail.
/// </summary>
public class EmailService : IEmailService
{
    private readonly string? smtpEmail  = Environment.GetEnvironmentVariable("SmtpEmail");
    private readonly string? smtpAppPassword = Environment.GetEnvironmentVariable("SmtpAppPassword");


    private void SendEmail(MimeMessage email)
    {
        using var smtpClient = new SmtpClient();
        try
        {
            this.ConfigureSmtpClient(smtpClient);
            smtpClient.Send(email);
        }
        finally
        {
            smtpClient.Disconnect(true);
        }
    }


    private void ConfigureSmtpClient(SmtpClient smtpClient)
    {
        if (string.IsNullOrEmpty(this.smtpEmail) || string.IsNullOrEmpty(this.smtpAppPassword))
        {
            throw new InvalidOperationException("SMTP email or password environment variables are not set.");
        }

        smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtpClient.Authenticate(this.smtpEmail, this.smtpAppPassword);
    }


    public void SendVerificationEmail(Member member)
    {
        var email = this.CreateVerificationEmail(member, GenerateVerificationLink(member));
        this.SendEmail(email);
    }


    private static string GenerateVerificationLink(Member member)
        => $"http://localhost:8080/api/verify/{member.VerificationToken}";


    private MimeMessage CreateVerificationEmail(Member member, string verificationLink)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(this.smtpEmail));
        email.To.Add(MailboxAddress.Parse(member.Email));
        email.Subject = "Konto-Verifizierung";


        var content = $"Bitte klicken Sie auf den folgenden Link, um Ihr Konto zu verifizieren: " +
                      $"<a href=\"{verificationLink}\">Link zum Verifizieren</a>";

        var htmlContent = $"<html><body><p style='font-weight: bold;'>{content}</p></body></html>";
        email.Body = new TextPart("html")
        {
            Text = htmlContent
        };

        return email;
    }
}
