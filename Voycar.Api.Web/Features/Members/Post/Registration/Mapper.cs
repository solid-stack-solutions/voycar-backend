namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.Security.Cryptography;
using Entities;

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
        DriversLicenseNumber = r.DriversLicenseNumber,
        IdCardNumber = r.IdCardNumber,
        VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(256))
    };
}
