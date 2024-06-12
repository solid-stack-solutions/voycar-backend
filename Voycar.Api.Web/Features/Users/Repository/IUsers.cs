namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;


public interface IUsers : Generic.Repository.IRepository<User>
{
    Task<User?> RetrieveByVerificationToken(string verificationToken);
    Task<User?> Retrieve(string attribute, string? value);
}
