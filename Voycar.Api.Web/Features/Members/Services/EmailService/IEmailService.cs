namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;
using MimeKit;
using Post.Registration;

public interface IEmailService
{
    void SendVerificationEmail(Member member);
}
