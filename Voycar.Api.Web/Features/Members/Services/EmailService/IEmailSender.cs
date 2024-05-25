namespace Voycar.Api.Web.Features.Members.Services.EmailService;

public interface IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message);
}
