using Mediators.Models;
using Mediators.Repository.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Repository;

public static class UserRegistrationExtensions
{
    public static IServiceCollection RegisterUserRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var useInMemoryDb = configuration.GetSection("UseInMemoryDatabase").Value == "True";
        var connectionString = configuration.GetSection("ConnectionStrings:UserDatabase").Value
            ?? throw new InvalidOperationException("ConnectionStrings:UserDatabase is not configured in appsettings.json");

        if (useInMemoryDb)
        {
            // Use connection string as database name for in-memory to ensure test isolation
            services.AddInMemoryUserEntityFrameworkStorage(connectionString);
        }
        else
        {
            services.AddSqlServerUserEntityFrameworkStorage(connectionString);
        }

        return services;
    }
}


