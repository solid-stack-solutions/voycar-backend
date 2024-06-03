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
public class MemberRepository : Generic.Repository.Repository<Member>, IMemberRepository
{
    private readonly VoycarDbContext _context;

    public MemberRepository(VoycarDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<Member?> Retrieve(string verificationToken)
    {
        return  this._context.Members.FirstOrDefaultAsync(
            member => member.VerificationToken == verificationToken);
    }

    public Task<Role?> RetrieveRoleId(Guid roleId)
    {
        return this._context.Roles.FirstOrDefaultAsync(
            role => role.Id == roleId);
    }
}
