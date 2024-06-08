namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;


public interface IMembers : Generic.Repository.IRepository<Member>
{
    Task<Member?> Retrieve(string verificationToken);
}
