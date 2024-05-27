namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


public class EmailService : IEmailService
{
    private readonly string? _smtpEmail  = Environment.GetEnvironmentVariable("SmtpEmail");
    private readonly string? _smtpAppPassword = Environment.GetEnvironmentVariable("SmtpAppPassword");


    public void SendVerificationEmail(Member member)
    {
        if (string.IsNullOrEmpty(this._smtpEmail) || string.IsNullOrEmpty(this._smtpAppPassword))
        {
            throw new InvalidOperationException("SMTP email or password environment variables are not set.");
        }

        var email = this.CreateVerificationEmail(member, this.GenerateVerificationLink(member));
        using var smtpClient = new SmtpClient();
        try
        {
            smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtpClient.Authenticate(this._smtpEmail, this._smtpAppPassword);
            smtpClient.Send(email);
        }
        finally
        {
            smtpClient.Disconnect(true);
        }
    }

    public string GenerateVerificationLink(Member member)
        => $"http://localhost:8080/api/verify/{member.VerificationToken}";

    public MimeMessage CreateVerificationEmail(Member member, string verificationLink )
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(this._smtpEmail));
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
