namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using Generic;

public class Reservation : Entity
{
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }

    [ForeignKey("Member")]
    public Guid MemberId { get; set; }
    public Member Member { get; set; }

    // ToDo 1:1 relationship to Car
}
