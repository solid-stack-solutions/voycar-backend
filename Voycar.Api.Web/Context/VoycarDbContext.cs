namespace Voycar.Api.Web.Context;

using Entities;
using Microsoft.EntityFrameworkCore;


public class VoycarDbContext : DbContext
{
    public VoycarDbContext(DbContextOptions<VoycarDbContext> options) : base(options) {}

    // Db sets for entities
    public DbSet<User> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<City> Cities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-one relationship between User and Member
        modelBuilder.Entity<Member>()
            .HasOne(m => m.User) // Each Member entity has one User entity
            .WithOne() // Each User entity is associated with one Member entity
            .HasForeignKey<Member>(m => m.Id) // Sets the foreign key to the Id property of Member, which is also the Id of the associated User
            .IsRequired();

        // Configure the one-to-many relationship between Role and User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role) // Each User entity has one Role entity
            .WithMany() // Each Role entity can be associated with multiple User entities
            .HasForeignKey(u => u.RoleId) // The RoleId property in User is a foreign key that references the Role entity
            .IsRequired();

        // Configure the one-to-many relationship between Reservation and Member
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Member) // Each Reservation has one Member
            .WithMany() // Each Member can be associated with multiple Reservations
            .HasForeignKey(r => r.MemberId) // MemberId is foreign key for Member
            .IsRequired();

        // Add default data to role table
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.NewGuid(), Name = "admin" },
            new Role { Id = Guid.NewGuid(), Name = "employee" },
            new Role { Id = Guid.NewGuid(), Name = "member" });
        base.OnModelCreating(modelBuilder);
    }
}
