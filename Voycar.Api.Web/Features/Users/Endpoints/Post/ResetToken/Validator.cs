namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetToken;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .WithName("generalErrors")
            .EmailAddress()
            .WithMessage("Email address is not valid")
            .WithName("generalErrors");
    }
}
