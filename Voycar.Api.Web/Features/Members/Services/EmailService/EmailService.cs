namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


/// <summary>
/// Service for sending verification emails to members.
///
/// Uses SMTP credentials from environment variables to send emails via Gmail.
/// </summary>
public class EmailService : IEmailService
{
    private readonly string? _smtpEmail  = Environment.GetEnvironmentVariable("SmtpEmail");
    private readonly string? _smtpAppPassword = Environment.GetEnvironmentVariable("SmtpAppPassword");


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
        if (string.IsNullOrEmpty(this._smtpEmail) || string.IsNullOrEmpty(this._smtpAppPassword))
        {
            throw new InvalidOperationException("SMTP email or password environment variables are not set.");
        }

        smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        smtpClient.Authenticate(this._smtpEmail, this._smtpAppPassword);
    }


    public void SendVerificationEmail(Member member)
    {
        var email = this.CreateVerificationEmail(member, GenerateVerificationLink(member));
        this.SendEmail(email);
    }


    public void SendPasswordResetEmail(User user)
    {
        var email = this.CreatePasswordResetEmail(user, GeneratePasswordResetLink());
        this.SendEmail(email);
    }


    // ToDo Link to Frontend must be added + VerificationToken must be attached
    private static string GenerateVerificationLink(Member member)
        => $"http://localhost:8080/verify/{member.VerificationToken}"; // FrontendLink?token={VerificationToken}


    // ToDo Link to Frontend must be added + PasswordResetToken must be attached
    private static string GeneratePasswordResetLink()
        => $"http://localhost:8080/reset-password"; // FrontendLink?token={PasswordResetToken}


    private MimeMessage CreateVerificationEmail(Member member, string verificationLink)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(this._smtpEmail));
        email.To.Add(MailboxAddress.Parse(member.User.Email));
        email.Subject = "Voycar-Konto-Verifizierung";


        var content = $"Bitte klicken Sie auf den folgenden Link, um Ihr Konto zu verifizieren: " +
                      $"<a href=\"{verificationLink}\">Link zum Verifizieren</a>";

        var htmlContent = $"<html><body><p style='font-weight: bold;'>{content}</p></body></html>";
        email.Body = new TextPart("html")
        {
            Text = htmlContent
        };

        return email;
    }


    private MimeMessage CreatePasswordResetEmail(User user, string passwordResetLink)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(this._smtpEmail));
        email.To.Add(MailboxAddress.Parse(user.Email));
        email.Subject = "Voycar-Passwort-Reset";


        var content = $"Bitte klicken Sie auf den folgenden Link, um Ihr Passwort zurückzusetzen: " +
                      $"<a href=\"{passwordResetLink}\">Link zum Passwort-Reset</a>";

        var htmlContent = $"<html><body><p style='font-weight: bold;'>{content}</p></body></html>";
        email.Body = new TextPart("html")
        {
            Text = htmlContent
        };

        return email;
    }
}
