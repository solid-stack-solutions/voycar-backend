namespace Voycar.Api.Web.Features.Members.Endpoints.Put.Personal;

using Entities;


public class Mapper : RequestMapper<Request, Member>
{
    public Member ToEntity(Request r, Member member)
    {
        // Create the Member entity
        return new Member
        {
            Id = member.Id,
            FirstName = r.FirstName,
            LastName = r.LastName,
            Street = r.Street,
            HouseNumber = r.HouseNumber,
            PostalCode = r.PostalCode,
            City = r.City,
            Country = r.Country,
            BirthDate = r.BirthDate,
            BirthPlace = r.BirthPlace,
            PhoneNumber = r.PhoneNumber,
            PlanId = member.PlanId
        };
    }
}
