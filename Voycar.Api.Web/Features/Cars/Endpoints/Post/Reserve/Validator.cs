namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using Repository;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(req => req.End - req.Begin)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("End-time is not after begin-time");
        this.RuleFor(req => this.Resolve<ICars>().Retrieve(req.CarId))
            .NotNull()
            .WithMessage("There is no car with the given ID");
        // ToDo check if car is already reserved in that time, return conflict?
    }
}
