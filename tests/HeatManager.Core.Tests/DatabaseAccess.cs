using HeatManager.Core.Db;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace HeatManager.Core.Tests;

public abstract class DatabaseAccess : IAsyncLifetime
{

    protected readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    //protected IUserService UserService { get; set; }
    protected HeatManagerDbContext _dbContext;

    public virtual async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var dbContextOptions = new DbContextOptionsBuilder<HeatManagerDbContext>()
            .UseNpgsql(_postgreSqlContainer.GetConnectionString(), o =>
                o.ConfigureDataSource(o =>
                {
                    o.EnableDynamicJson();
                }))
            .Options;

        var str = _postgreSqlContainer.GetConnectionString();

        _dbContext = new HeatManagerDbContext(dbContextOptions);

        await _dbContext.Database.EnsureCreatedAsync();

    }

    public virtual Task DisposeAsync()
    {
        return _postgreSqlContainer.StopAsync();
    }

}