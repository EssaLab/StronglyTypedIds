// using EssaLab.StronglyTypedIds.Convertors.EntityFrameworkCore;

using EssaLab.StronglyTypedIds.Convertors.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestApp.Domain;

namespace TestApp.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; init; }
    // public DbSet<Customer> Customers { get; init; }
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