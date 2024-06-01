namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;


public interface IMemberRepository
{
    Task CreateAsync(Member member);
    Task<User?> GetAsync(Post.Registration.Request request);
    Task<User?> GetAsync(Post.Login.Request request);
    Task<Member?> GetAsync(string verificationToken);
    Task<Member?> GetAsync(Guid userId);
    Task<Role?> GetRoleAsync(int roleId);

    Task SaveAsync();
}
