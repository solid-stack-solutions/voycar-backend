namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;


public interface IEmailService
{
    void SendVerificationEmail(Member member);
}
