namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;
using Context;


/// <summary>
/// Repository for managing member data.
///
/// Provides methods for creating members, retrieving members by verification token,
/// and saving changes to the database.
/// </summary>
public class Members : Generic.Repository.Repository<Member>, IMembers
{
    public Members(VoycarDbContext context) : base(context) {}
}
