namespace Voycar.Api.Web.Features.Cars.Endpoints.Get.Available;

using Stations.Repository;

public class Validator : Validator<Request>
{
    public Validator()
    {
        this.RuleFor(req => req.End - req.Begin)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("End-time is not after begin-time");
        this.RuleFor(req => this.Resolve<IStations>().Retrieve(req.StationId))
            .NotNull()
            .WithMessage("There is no station with the given ID");
    }
}
