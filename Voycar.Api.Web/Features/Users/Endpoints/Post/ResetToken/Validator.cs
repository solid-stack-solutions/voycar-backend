namespace Voycar.Api.Web.Features.Members.Post.ForgotPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.Email)
            .NotEmpty().WithMessage("your email is required!")
            .EmailAddress().WithMessage("not a valid email address");
    }
}
