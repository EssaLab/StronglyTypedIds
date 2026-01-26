using Microsoft.EntityFrameworkCore;
using StronglyTypedIds.EntityFrameworkCore;

namespace TestApp.Infrastructure;

public class AppDbContext : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.AddStronglyTypedIdConventions();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}