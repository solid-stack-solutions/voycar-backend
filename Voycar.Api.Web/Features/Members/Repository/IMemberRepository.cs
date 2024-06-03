namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;


public interface IMemberRepository : Generic.Repository.IRepository<Member>
{
    Task<Member?> Retrieve(string verificationToken);
    Task<Role?> RetrieveRoleId(Guid roleId);
}
