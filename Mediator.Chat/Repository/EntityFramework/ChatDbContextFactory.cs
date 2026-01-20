using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Design-time factory for creating ChatDbContext.
/// This is used by EF Core tools (like migrations) during design time.
/// </summary>
public class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
        
        // Use a default connection string for design-time operations
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=ChatDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new ChatDbContext(optionsBuilder.Options);
    }
}
