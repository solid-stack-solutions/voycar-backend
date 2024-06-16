namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Generic;

public class Reservation : Entity
{
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }

    [ForeignKey("Member")]
    public Guid MemberId { get; set; }
    [JsonIgnore]
    public Member Member { get; set; }

    [ForeignKey("Car")]
    public Guid CarId { get; set; }
    [JsonIgnore]
    public Car Car { get; set; }
}
