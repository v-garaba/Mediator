# Entity Framework Storage Implementation

This document describes the Entity Framework implementation for persistent message storage in the Mediator Chat System.

## Overview

The Entity Framework storage implementation provides database persistence for chat messages using SQL Server. It replaces the in-memory `MessageStorage` with a database-backed solution while maintaining the same `IStorage<MessageRef, ChatMessage>` interface.

## Architecture

### Key Components

1. **ChatDbContext** - Entity Framework DbContext that manages database connections and entity configurations
2. **ChatMessageEntity** - Database entity model representing a chat message
3. **EntityFrameworkMessageStorage** - Implementation of `IStorage<MessageRef, ChatMessage>` using Entity Framework
4. **ChatDbContextFactory** - Design-time factory for EF Core tooling and migrations

### File Structure

```
Mediators/Repository/EntityFramework/
├── ChatDbContext.cs                              # DbContext configuration
├── ChatDbContextFactory.cs                       # Design-time DbContext factory
├── EntityFrameworkMessageStorage.cs              # Storage implementation
├── EntityFrameworkRegistrationExtensions.cs      # Dependency injection extensions
└── Entities/
    └── ChatMessageEntity.cs                      # Database entity model
```

## Configuration

### Connection String

The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ChatDatabase": "Server=(localdb)\\mssqllocaldb;Database=ChatDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "UseInMemoryDatabase": false
}
```

### Database Provider Options

The implementation supports two database providers:

1. **SQL Server** (Production) - Set `UseInMemoryDatabase: false`
2. **In-Memory** (Testing) - Set `UseInMemoryDatabase: true`

## Database Schema

### Messages Table

| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier (PK) | Unique message identifier |
| SenderId | uniqueidentifier | User who sent the message |
| Content | nvarchar(4000) | Message text content |
| MessageType | int | Type of message (0=Public, 1=Private, 2=System) |
| TargetUserId | uniqueidentifier (nullable) | Recipient for private messages |
| Timestamp | datetimeoffset | Message creation timestamp |

### Indexes

- `IX_Messages_SenderId` - Index on SenderId for fast sender lookup
- `IX_Messages_Timestamp` - Index on Timestamp for chronological queries
- `IX_Messages_TargetUserId_MessageType` - Composite index for private message queries

## Usage

### Registration

The Entity Framework storage is registered in `Program.cs`:

```csharp
var serviceProvider = new ServiceCollection()
    .AddLogging(...)
    .RegisterRepositories() // Other repositories
    .RegisterRequestHandlers()
    .RegisterNotificationHandlers()
    .AddSingleton<IMediator, ChatMediator>()
    .AddSingleton<ChatRoom>()
    .AddEntityFrameworkStorage(useInMemoryDb, connectionString)
    .BuildServiceProvider();

// Ensure database is created
if (!useInMemoryDb)
{
    await serviceProvider.EnsureDatabaseCreatedAsync();
}
```

### Extension Methods

**AddEntityFrameworkStorage**
```csharp
services.AddEntityFrameworkStorage(bool useInMemory, string connectionString)
```
Registers EF storage with either SQL Server or In-Memory provider.

**AddSqlServerEntityFrameworkStorage**
```csharp
services.AddSqlServerEntityFrameworkStorage(string connectionString)
```
Explicitly registers SQL Server provider with retry logic and connection resilience.

**AddInMemoryEntityFrameworkStorage**
```csharp
services.AddInMemoryEntityFrameworkStorage(string databaseName = "ChatDb")
```
Registers In-Memory provider for testing scenarios.

**EnsureDatabaseCreatedAsync**
```csharp
await serviceProvider.EnsureDatabaseCreatedAsync();
```
Creates the database if it doesn't exist (for development).

**MigrateDatabaseAsync**
```csharp
await serviceProvider.MigrateDatabaseAsync();
```
Applies pending migrations (for production deployments).

## Database Migrations

### Creating a Migration

```bash
dotnet ef migrations add MigrationName
```

### Applying Migrations

```bash
dotnet ef database update
```

### Removing Last Migration

```bash
dotnet ef migrations remove
```

### Listing Migrations

```bash
dotnet ef migrations list
```

## Implementation Details

### Entity Mapping

The storage implementation maps between domain models (`ChatMessage`) and database entities (`ChatMessageEntity`):

**Domain Model → Entity (MapToEntity)**
```csharp
ChatMessage -> ChatMessageEntity
{
    Id = model.Id.Id,
    SenderId = model.SenderId.Id,
    Content = model.Content,
    MessageType = (int)model.Type,
    TargetUserId = model.TargetUserId?.Id,
    Timestamp = model.Timestamp
}
```

**Entity → Domain Model (MapToModel)**
```csharp
ChatMessageEntity -> ChatMessage
{
    SenderId = new UserRef { Id = entity.SenderId },
    Content = entity.Content,
    Type = (MessageType)entity.MessageType,
    TargetUserId = entity.TargetUserId.HasValue 
        ? new UserRef { Id = entity.TargetUserId.Value } 
        : null,
    Id = new MessageRef { Id = entity.Id },
    Timestamp = entity.Timestamp
}
```

### Change Tracking

The `SetAsync` method handles entity tracking to avoid conflicts:

1. First checks the EF change tracker for existing tracked entities
2. If found, updates the tracked entity's values
3. If not tracked, checks database existence
4. Performs INSERT or UPDATE accordingly

This approach prevents "already tracked" exceptions when the same entity is modified multiple times in one scope.

### Connection Resilience

The SQL Server configuration includes automatic retry logic:

```csharp
options.UseSqlServer(connectionString, sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
    
    sqlOptions.CommandTimeout(30);
});
```

## Features

✅ **Persistent Storage** - Messages are stored in SQL Server database
✅ **Entity Framework Core 9.0** - Latest EF Core features and performance
✅ **Connection Resilience** - Automatic retry on transient failures
✅ **In-Memory Testing** - Support for in-memory database for unit tests
✅ **Migrations** - Database schema versioning with EF Core migrations
✅ **Optimized Queries** - Strategic indexes for common query patterns
✅ **Async Operations** - Fully asynchronous database operations
✅ **Proper Entity Tracking** - Handles change tracking edge cases
✅ **Design-Time Support** - Factory pattern for EF Core tooling

## Performance Considerations

1. **AsNoTracking** - Used for read-only queries to improve performance
2. **Indexes** - Strategic indexes on SenderId, Timestamp, and TargetUserId
3. **Batch Operations** - SaveChanges batches multiple operations
4. **Connection Pooling** - Automatic connection pooling by EF Core
5. **Compiled Queries** - EF Core automatically compiles frequently-used queries

## Testing

To test with in-memory database, set in `appsettings.json`:

```json
{
  "UseInMemoryDatabase": true
}
```

This allows testing without requiring SQL Server installation.

## Production Deployment

For production deployment:

1. Update connection string in `appsettings.json` or environment variables
2. Run migrations: `dotnet ef database update`
3. Set `UseInMemoryDatabase: false`
4. Ensure SQL Server is accessible with proper credentials

## Troubleshooting

### Common Issues

**Issue: "Cannot track entity" exception**
- Solution: The implementation handles this with Local collection check

**Issue: Database not created**
- Solution: Call `EnsureDatabaseCreatedAsync()` or use migrations

**Issue: Connection timeout**
- Solution: Check connection string and SQL Server accessibility

**Issue: Migration conflicts**
- Solution: Remove last migration and recreate after resolving conflicts

## Future Enhancements

Potential improvements for the implementation:

1. Add user and user notification entity storage
2. Implement soft delete functionality
3. Add audit logging for message changes
4. Implement read/unread message tracking
5. Add message search with full-text indexing
6. Implement message archiving strategy
7. Add database sharding for scalability
8. Implement CQRS pattern with read replicas

## Dependencies

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.10" />
```

## Conclusion

The Entity Framework implementation provides a robust, production-ready storage solution for the chat system while maintaining compatibility with the existing storage interface. It supports both SQL Server for production and in-memory database for testing, with proper connection resilience and performance optimizations.
