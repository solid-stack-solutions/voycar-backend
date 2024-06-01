namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;
using Microsoft.EntityFrameworkCore;
using Post.Registration;
using Context;


/// <summary>
/// Repository for managing member data.
///
/// Provides methods for creating members, retrieving members by email or verification token,
/// and saving changes to the database.
/// </summary>
public class MemberRepository : IMemberRepository
{
    private readonly VoycarDbContext _dbContext;


    public MemberRepository(VoycarDbContext dbContext)
    {
        this._dbContext = dbContext;
    }


    public async Task CreateAsync(Member member)
    {
        await this._dbContext.Members.AddAsync(member);
        await this._dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get the Member which has the same E-Mail as in the given request.
    /// </summary>
    public async Task<User?> GetAsync(Post.Registration.Request request)
    {
        return await this._dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == request.Email);
    }

    /// <summary>
    /// Get the Member which has the same E-Mail as in the given request.
    /// </summary>
    public async Task<User?> GetAsync(Post.Login.Request request)
    {
        return await this._dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == request.Email);
    }

    /// <summary>
    /// Get the Member which has the same VerificationToken as in the given request.
    /// </summary>
    public async Task<Member?> GetAsync(string verificationToken)
    {
         return await this._dbContext.Members.FirstOrDefaultAsync(
             member => member.VerificationToken == verificationToken);
    }

    public async Task<Member?> GetAsync(Guid userId)
    {
        return await this._dbContext.Members.FirstOrDefaultAsync(
            member => member.UserId == userId);
    }

    /// <summary>
    /// Get the Role of a User by RoleId.
    /// </summary>
    public async Task<Role?> GetRoleAsync(int roleId)
    {
        return await this._dbContext.Roles.FirstOrDefaultAsync(
            role => role.Id == roleId);
    }
    public async Task SaveAsync() => await this._dbContext.SaveChangesAsync();
}
