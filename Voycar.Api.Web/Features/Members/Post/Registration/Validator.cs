namespace Voycar.Api.Web.Features.Members.Post.Registration;

/// <summary>
/// Validator class for user registration requests.
///
/// Validates that a valid email and nonempty password have been sent.
/// </summary>
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
