using Mediators.Models;
using Mediators.Repository.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Repository;

public static class ChatRegistrationExtensions
{
    public static IServiceCollection RegisterChatRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var useInMemoryDb = configuration.GetSection("UseInMemoryDatabase").Value == "True";
        var connectionString = configuration.GetSection("ConnectionStrings:ChatDatabase").Value
            ?? throw new InvalidOperationException("ConnectionStrings:ChatDatabase is not configured in appsettings.json");

            services.AddEntityFrameworkStorage(useInMemoryDb, connectionString);

        return services;
    }
}


