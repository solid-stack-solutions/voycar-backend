namespace Voycar.Api.Web.Context;

using Microsoft.EntityFrameworkCore;

public class VoycarDbContext : DbContext
{
    public VoycarDbContext(DbContextOptions<VoycarDbContext> options) : base(options){ }

}
