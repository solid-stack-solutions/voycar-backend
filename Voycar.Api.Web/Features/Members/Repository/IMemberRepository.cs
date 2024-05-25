namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;
using Post.Registration;

public interface IMemberRepository
{
    public Task CreateAsync(Member member);
    public Task<Member?> GetAsync(Request request);
    public Task<Member?> GetAsync(string verificationToken);
    public Task SafeAsync();
}
