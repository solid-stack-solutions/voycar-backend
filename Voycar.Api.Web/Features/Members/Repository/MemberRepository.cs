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
    private readonly VoycarDbContext dbContext;


    public MemberRepository(VoycarDbContext dbContext)
    {
        this.dbContext = dbContext;
    }


    public async Task CreateAsync(Member member)
    {
        await this.dbContext.Members.AddAsync(member);
        await this.dbContext.SaveChangesAsync();
    }


    public async Task<Member?> GetAsync(Request request)
        => await this.dbContext.Members.FirstOrDefaultAsync(
            member => member.Email == request.Email);


    public async Task<Member?> GetAsync(string verificationToken)
        => await this.dbContext.Members.FirstOrDefaultAsync(
            member => member.VerificationToken == verificationToken);


    public async Task SafeAsync() => await this.dbContext.SaveChangesAsync();
}
