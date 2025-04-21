using HeatManager.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace HeatManager.Core.Db;

public class HeatManagerDbContext(DbContextOptions<HeatManagerDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Project>(p => p
                .Property(p => p.ProjectData)
                .HasColumnType("jsonb")
            );
    }
}