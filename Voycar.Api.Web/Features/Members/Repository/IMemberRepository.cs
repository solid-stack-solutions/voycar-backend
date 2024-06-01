namespace Voycar.Api.Web.Features.Members.Repository;

using Entities;


public interface IMemberRepository
{
    Task CreateAsync(Member member);
    Task<User?> GetAsync(Post.Registration.Request request);
    Task<User?> GetAsync(Post.Login.Request request);
    Task<User?> GetAsync(Post.ForgotPassword.Request request);
    Task<Member?> GetAsync(string verificationToken);
    Task<User?> GetPrtAsync(string passwordResetToken);
    Task<Member?> GetAsync(Guid userId);
    Task<Role?> GetRoleAsync(int roleId);

    Task SaveAsync();
}
