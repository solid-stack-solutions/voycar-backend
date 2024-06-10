namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using Generic;

public class Station : Entity
{
    public string Name { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    [ForeignKey("City")]
    public Guid CityId { get; set; }
    public City City { get; set; }
}
