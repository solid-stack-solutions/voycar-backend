namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;


public interface IEmailService
{
    void SendVerificationEmail(User user);
    void SendPasswordResetEmail(User user);
}
