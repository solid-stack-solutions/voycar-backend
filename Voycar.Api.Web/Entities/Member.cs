namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Generic;

public class Member : Entity
{
    // Personal information
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateOnly BirthDate { get; set; }
    public string BirthPlace { get; set; }

    // Contact information
    public string? PhoneNumber { get; set; }

    // Plan information foreign key
    [ForeignKey("Plan")]
    public Guid PlanId { get; set; }
    [JsonIgnore]
    public Plan Plan { get; set; }

}
