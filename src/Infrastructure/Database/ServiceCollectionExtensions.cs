using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Database
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCrmPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CrmDb");

            services.AddDbContext<CrmDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
