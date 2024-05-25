namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;
using Microsoft.EntityFrameworkCore;
using Post.Registration;
using Voycar.Api.Web.Context;


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
        => await this._dbContext.Members.FirstOrDefaultAsync(mem => mem.Email == request.Email);
}
