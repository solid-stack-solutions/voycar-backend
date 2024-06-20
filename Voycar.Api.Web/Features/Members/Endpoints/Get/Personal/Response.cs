namespace Voycar.Api.Web.Features.Members.Endpoints.Get.Personal;

public class Response
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
    public string PhoneNumber { get; set; }

    // Plan information
    public string PlanName { get; set; }
}
