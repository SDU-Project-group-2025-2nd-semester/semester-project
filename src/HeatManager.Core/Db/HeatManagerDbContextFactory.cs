using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HeatManager.Core.Db;

public class HeatManagerDbContextFactory : IDesignTimeDbContextFactory<HeatManagerDbContext>
{

    public HeatManagerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HeatManagerDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Database=heatManager;Username=postgres;Password=postgres");

        return new HeatManagerDbContext(optionsBuilder.Options);
    }
}