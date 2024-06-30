namespace Voycar.Api.Web.Service;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Entities;

/// <summary>
/// Service for sending verification and password reset emails to users.
///
/// Uses SMTP credentials from environment variables to send emails via Gmail.
/// </summary>
public class EmailService : IEmailService
{
    private readonly string? SmtpAppPassword = Environment.GetEnvironmentVariable("SmtpAppPassword");

    private const string SmtpEmail = "voycar.dev@gmail.com";
    private const string SmtpHostAddress = "smtp.gmail.com";
    private const int SmtpPort = 587;


    public void SendVerificationEmail(User user)
    {
        var email = this.CreateVerificationEmail(user, CreateVerificationLink(user));
        this.SendEmail(email);
    }


    public void SendPasswordResetEmail(User user)
    {
        var email = this.CreatePasswordResetEmail(user, CreatePasswordResetLink(user));
        this.SendEmail(email);
    }


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
        if (string.IsNullOrEmpty(SmtpEmail) || string.IsNullOrEmpty(this.SmtpAppPassword))
        {
            throw new InvalidOperationException("SMTP email or password environment variables are not set.");
        }

        smtpClient.Connect(SmtpHostAddress, SmtpPort, SecureSocketOptions.StartTls);
        smtpClient.Authenticate(SmtpEmail, this.SmtpAppPassword);
    }


    // ToDo Link to Frontend must be added + VerificationToken must be attached
    private static string CreateVerificationLink(User user)
    {
        return $"http://localhost:8080/auth/verify/{user.VerificationToken}"; // FrontendLink?token={VerificationToken}
    }


    // ToDo Link to Frontend must be added + PasswordResetToken must be attached
    private static string CreatePasswordResetLink(User user)
    {
        return $"http://localhost:5173/forgot-password/{user.PasswordResetToken}"; // FrontendLink?token={PasswordResetToken}
    }


    private MimeMessage CreateVerificationEmail(User user, string verificationLink)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(SmtpEmail));
        email.To.Add(MailboxAddress.Parse(user.Email));
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
        email.From.Add(MailboxAddress.Parse(SmtpEmail));
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
