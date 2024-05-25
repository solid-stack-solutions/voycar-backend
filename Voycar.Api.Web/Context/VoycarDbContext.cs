namespace Voycar.Api.Web.Context;

using Entities;
using Microsoft.EntityFrameworkCore;

public class VoycarDbContext : DbContext
{
    public VoycarDbContext(DbContextOptions<VoycarDbContext> options) : base(options){ }

    public DbSet<Member> Members { get; set; }
}
