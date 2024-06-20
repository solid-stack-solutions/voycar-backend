namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Generic;

public class Station : Entity
{
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    [ForeignKey("City")]
    public Guid CityId { get; set; }
    [JsonIgnore]
    public City City { get; set; }
}
