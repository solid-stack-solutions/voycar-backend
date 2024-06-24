namespace Voycar.Api.Web.Features.Users.Endpoints.Get.Verify;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.VerificationToken)
            .NotEmpty()
            .WithMessage("Verification token is required")
            .WithName("generalErrors");
    }
}
