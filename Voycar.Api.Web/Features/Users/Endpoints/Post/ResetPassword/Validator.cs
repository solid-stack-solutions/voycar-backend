namespace Voycar.Api.Web.Features.Members.Post.ResetPassword;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.PasswordResetToken)
            .NotEmpty().WithMessage("your password reset token is required!");

        this.RuleFor(request => request.Password)
            .NotEmpty().WithMessage("your password is required!");
    }
}
