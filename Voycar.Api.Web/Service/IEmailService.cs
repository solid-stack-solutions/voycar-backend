namespace Voycar.Api.Web.Service;

using Entities;

public interface IEmailService
{
    void SendVerificationEmail(User user);
    void SendPasswordResetEmail(User user);
}
