namespace Voycar.Api.Web.Features.Reservation.Endpoints.Get.Personal;

using Entities;


public class Response
{
    public List<Reservation> expired { get; set; }
    public List<Reservation> active { get; set; }
    public List<Reservation> planned { get; set; }
}
