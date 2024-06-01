namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;
using Post.Registration;

public interface IMemberRepository
{
    Task CreateAsync(Member member);
    Task<User?> GetAsync(Request request);
    Task<Member?> GetAsync(string verificationToken);
    Task SaveAsync();
}
