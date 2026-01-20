using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Design-time factory for creating UserDbContext.
/// This is used by EF Core tools (like migrations) during design time.
/// </summary>
public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
        
        // Use a default connection string for design-time operations
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=UserDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new UserDbContext(optionsBuilder.Options);
    }
}
