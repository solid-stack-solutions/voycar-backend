namespace Voycar.Api.Web.Context;

using Entities;
using Microsoft.EntityFrameworkCore;

public class VoycarDbContext : DbContext
{
    public VoycarDbContext(DbContextOptions<VoycarDbContext> options) : base(options) {}

    // db sets for entities
    public DbSet<User> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Role> Roles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-one relationship between User and Member
        modelBuilder.Entity<Member>()
            .HasOne(m => m.User)
            .WithOne()
            .HasForeignKey<Member>(m => m.UserId)
            .IsRequired();

        // Configure the one-to-many relationship between Role and User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
