namespace Voycar.Api.Web.Features.Members.Services.EmailService;

using Entities;
using Post.Registration;

public interface IEmailService
{
    public void SendEmail(Member member);
}
