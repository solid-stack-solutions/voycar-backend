namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using System.ComponentModel;

// Very similar to Reservation entity, but without ID and navigation properties
public class Request
{
    // Provide default values to simplify swagger usage
    [DefaultValue("00000000-0000-0000-0000-000000000000")]
    public Guid CarId { get; set; }
    [DefaultValue("00000000-0000-0000-0000-000000000000")]
    public Guid MemberId { get; set; }
    [DefaultValue("2000-01-01T08:00:00.000Z")]
    public DateTime Begin { get; set; }
    [DefaultValue("2000-01-01T18:00:00.000Z")]
    public DateTime End { get; set; }
}
