using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Design-time factory for ChatDbContext to support migrations
/// </summary>
public class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
        
        // Use a default connection string for migrations
        // This will be replaced at runtime with the actual connection string from appsettings.json
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=ChatDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new ChatDbContext(optionsBuilder.Options);
    }
}
