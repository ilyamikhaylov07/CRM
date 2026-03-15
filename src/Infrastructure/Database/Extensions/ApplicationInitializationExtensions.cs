namespace Crm.Infrastructure.Database.Extensions;
public static class ApplicationInitializationExtensions
{
    public static async Task SeedRolesAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
        await seeder.SeedAsync();
    }
}
