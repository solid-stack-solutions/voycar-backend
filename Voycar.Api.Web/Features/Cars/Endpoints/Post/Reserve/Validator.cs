namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using Repository;
using Users.Repository;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(req => req.End - req.Begin)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("End-time is not after begin-time")
            .WithName("generalErrors");
        this.RuleFor(req => this.Resolve<ICars>().Retrieve(req.CarId))
            .NotNull()
            .WithMessage("There is no car with the given ID")
            .WithName("generalErrors");
        this.RuleFor(req => this.Resolve<IUsers>().Retrieve(req.UserId))
            .NotNull()
            .WithMessage("There is no user with the given ID")
            .WithName("generalErrors");
    }
}
