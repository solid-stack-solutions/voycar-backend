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


    public async Task<Member?> GetAsync(Request request)
    {
        return await this._dbContext.Members.FirstOrDefaultAsync(
            member => member.Email == request.Email);
    }


    public async Task<Member?> GetAsync(string verificationToken)
    {
         return await this._dbContext.Members.FirstOrDefaultAsync(
             member => member.VerificationToken == verificationToken);
    }


    public async Task SaveAsync() => await this._dbContext.SaveChangesAsync();
}
