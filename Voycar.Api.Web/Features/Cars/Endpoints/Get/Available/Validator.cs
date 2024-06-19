namespace Voycar.Api.Web.Features.Cars.Endpoints.Get.Available;

using Stations.Repository;

public class Validator : Validator<Request>
{
    public Validator(IStations stationRepository)
    {
        this.RuleFor(req => stationRepository.Retrieve(req.StationId))
            .NotNull().WithMessage("there is no station with such id");
        this.RuleFor(req => req.End.Subtract(req.Begin))
            .GreaterThan(TimeSpan.Zero).WithMessage("end is not after begin");
    }
}
