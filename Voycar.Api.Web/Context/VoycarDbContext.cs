namespace Voycar.Api.Web.Context;

using Entities;
using Microsoft.EntityFrameworkCore;

public class VoycarDbContext : DbContext
{
    public VoycarDbContext(DbContextOptions<VoycarDbContext> options) : base(options) {}

    // db sets for entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<Member> Members { get; set; }
}
