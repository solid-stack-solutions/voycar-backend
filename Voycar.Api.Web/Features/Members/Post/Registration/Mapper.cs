namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Entities;
using RandomString4Net;

public class Mapper : Mapper<Request, Response, Member>
{

    public override Member ToEntity(Request r) => new()
    {
        Email = r.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password),
        FirstName = r.FirstName,
        LastName = r.LastName,
        Street = r.Street,
        HouseNumber = r.HouseNumber,
        PostalCode = r.PostalCode,
        Place = r.Place,
        BirthDate = r.BirthDate,
        BirthPlace = r.BirthPlace,
        PhoneNumber = r.PhoneNumber,
        IdCardNumber = r.IdCardNumber,
        DriversLicenseNumber = r.DriversLicenseNumber,
        VerificationToken = RandomString.GetString(Types.ALPHANUMERIC_MIXEDCASE, 256)
    };
}
