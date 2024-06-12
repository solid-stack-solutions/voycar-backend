namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Login;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.Email)
            .NotEmpty().WithMessage("your email is required!")
            .EmailAddress().WithMessage("not a valid email address");

        this.RuleFor(request => request.Password)
            .NotEmpty().WithMessage("your password is required!");
    }
}
