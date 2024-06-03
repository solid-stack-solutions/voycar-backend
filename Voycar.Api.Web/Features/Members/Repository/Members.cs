namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;
using Microsoft.EntityFrameworkCore;
using Context;


/// <summary>
/// Repository for managing member data.
///
/// Provides methods for creating members, retrieving members by verification token,
/// and saving changes to the database.
/// </summary>
public class Members : Generic.Repository.Repository<Member>, IMembers
{
    private readonly VoycarDbContext _context;

    public Members(VoycarDbContext context) : base(context)
    {
        _context = context;
    }


    public Task<Member?> Retrieve(string verificationToken)
    {
        return  this._context.Members.FirstOrDefaultAsync(
            member => member.VerificationToken == verificationToken);
    }

    public Task<Role?> RetrieveRole(Guid roleId)
    {
        return this._context.Roles.FirstOrDefaultAsync(
            role => role.Id == roleId);
    }
}
