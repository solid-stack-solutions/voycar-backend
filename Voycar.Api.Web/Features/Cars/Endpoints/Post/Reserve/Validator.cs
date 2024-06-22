namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using Members.Repository;
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
        this.RuleFor(req => this.Resolve<IMembers>().Retrieve(req.MemberId))
            .NotNull()
            .WithMessage("There is no member with the given ID");
    }
}
