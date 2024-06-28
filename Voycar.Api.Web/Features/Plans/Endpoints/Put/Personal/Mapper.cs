namespace Voycar.Api.Web.Features.Plans.Endpoints.Put.Personal;

using Entities;


public class Mapper : RequestMapper<Request, Member>
{
    public Member ToEntity(Request r, Member member)
    {
        // Create the Member entity
        return new Member
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Street = member.Street,
            HouseNumber = member.HouseNumber,
            PostalCode = member.PostalCode,
            City = member.City,
            Country = member.Country,
            BirthDate = member.BirthDate,
            BirthPlace = member.BirthPlace,
            PhoneNumber = member.PhoneNumber,
            PlanId = r.PlanId
        };
    }
}
