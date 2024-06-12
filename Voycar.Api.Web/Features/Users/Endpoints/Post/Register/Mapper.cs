namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Entities;


public class Mapper : Mapper<Request, Response, Member>
{
    public override Member ToEntity(Request r)
    {
        // Create the Member entity
        return new Member
        {
            Id = Guid.NewGuid(), // Set ID here, to reference it in the User Entity later
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
        };
    }
}
