using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Crm.Tests.Integration;

public sealed class PostgreSqlDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public CrmDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new CrmDbContext(options);
    }
}
