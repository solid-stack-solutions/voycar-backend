namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.PasswordResetToken)
            .NotEmpty()
            .WithMessage("Password reset token is required")
            .WithName("generalErrors");

        this.RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .WithName("generalErrors");
    }
}
