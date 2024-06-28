namespace Voycar.Api.Web.Features.Plans.Endpoints.Put.Personal;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(request => request.PlanId)
            .NotEmpty()
            .WithMessage("Plan id is required")
            .WithName("generalErrors");
    }
}


