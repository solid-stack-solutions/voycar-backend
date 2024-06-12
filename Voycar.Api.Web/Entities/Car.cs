namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Generic;

public class Car : Entity
{
    public string LicensePlate { get; set; }
    public short PS { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public short BuildYear { get; set; }
    public string Type { get; set; }
    public short Seats { get; set; }

    [ForeignKey("Station")]
    public Guid StationId { get; set; }
    [JsonIgnore]
    public Station Station { get; set; }
}
