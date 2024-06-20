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
    public DbSet<Station> Stations { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Plan> Plans { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-one relationship between User and Member
        modelBuilder.Entity<User>()
            .HasOne(u => u.Member) // Each User entity has one Member entity
            .WithOne() // Each Member entity is associated with one User entity
            .HasForeignKey<User>(u => u.MemberId) // The MemberId property in User is a foreign key that references the Member entity
            .IsRequired(false);

        // Configure the one-to-many relationship between Role and User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role) // Each User entity has one Role entity
            .WithMany() // Each Role entity can be associated with multiple User entities
            .HasForeignKey(u => u.RoleId) // The RoleId property in User is a foreign key that references the Role entity
            .IsRequired();

        // One-to-many relationship between Reservation and Member
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Member)
            .WithMany()
            .HasForeignKey(r => r.MemberId)
            .IsRequired();

        // One-to-many relationship between Reservation and Car
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Car)
            .WithMany()
            .HasForeignKey(r => r.CarId)
            .IsRequired();

        // One-to-many relationship between Station and City
        modelBuilder.Entity<Station>()
            .HasOne(s => s.City)
            .WithMany()
            .HasForeignKey(s => s.CityId)
            .IsRequired();

        // One-to-many relationship between Car and Station
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Station)
            .WithMany()
            .HasForeignKey(c => c.StationId)
            .IsRequired();

        // Add default data

        var bremenCity = new City { Id = Guid.NewGuid(), Country = "Germany", Name = "Bremen" };
        modelBuilder.Entity<City>().HasData(bremenCity);

        // Two stations are useful for some tests, e.g. checking for available cars at a station
        var trainStationStation = new Station { Id = Guid.NewGuid(), Name = "Bahnhof",   Latitude = 53.0850, Longitude = 8.8153, CityId = bremenCity.Id };
        var airportStation      = new Station { Id = Guid.NewGuid(), Name = "Flughafen", Latitude = 53.0547, Longitude = 8.7849, CityId = bremenCity.Id };
        modelBuilder.Entity<Station>().HasData(trainStationStation, airportStation);

        modelBuilder.Entity<Car>().HasData(
            new Car { Id = Guid.NewGuid(), LicensePlate = "HB KA 437",  PS = 510, Brand = "Porsche", Model = "911 GT3",            BuildYear = 2024, Type = "Sportscar", Seats = 2, StationId = trainStationStation.Id },
            new Car { Id = Guid.NewGuid(), LicensePlate = "HB JB 217",  PS = 570, Brand = "Nissan",  Model = "Mk4 GTR",            BuildYear = 2016, Type = "Sportscar", Seats = 2, StationId = airportStation.Id      },
            new Car { Id = Guid.NewGuid(), LicensePlate = "HB NR 6385", PS = 258, Brand = "Toyota",  Model = "GR Supra",           BuildYear = 2019, Type = "Sportscar", Seats = 2, StationId = trainStationStation.Id },
            new Car { Id = Guid.NewGuid(), LicensePlate = "HB KL 12",   PS = 570, Brand = "Audi",    Model = "R8 4S",              BuildYear = 2020, Type = "Sportscar", Seats = 2, StationId = trainStationStation.Id },
            new Car { Id = Guid.NewGuid(), LicensePlate = "HB JH 420",  PS = 808, Brand = "Dodge",   Model = "SRT Hellcat Redeye", BuildYear = 2023, Type = "Musclecar", Seats = 4, StationId = trainStationStation.Id });

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.NewGuid(), Name = "admin" },
            new Role { Id = Guid.NewGuid(), Name = "employee" },
            new Role { Id = Guid.NewGuid(), Name = "member" });

        modelBuilder.Entity<Plan>().HasData(
            new Plan { Id = Guid.NewGuid(), Name = "basic",     MonthlyPrice = 10.0f, HourlyPrice = 15.0f },
            new Plan { Id = Guid.NewGuid(), Name = "reduced",   MonthlyPrice = 20.0f, HourlyPrice = 12.5f },
            new Plan { Id = Guid.NewGuid(), Name = "exclusive", MonthlyPrice = 40.0f, HourlyPrice = 10.0f });

        // Configure when default data should be used
        base.OnModelCreating(modelBuilder);
    }
}
