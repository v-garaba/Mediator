# Entity Framework Core Integration

This folder contains the Entity Framework Core implementation for persisting chat messages to a SQL Server database.

## Components

### ChatDbContext
The EF Core DbContext that manages the database connection and entity mapping.

### ChatMessageEntity
The database entity that maps to the Messages table.

### EntityFrameworkMessageStorage
Implementation of `IStorage<MessageRef, ChatMessage>` that uses EF Core for persistence.

### EntityFrameworkRegistrationExtensions
Extension methods for registering EF Core services with dependency injection.

### ChatDbContextFactory
Design-time factory for EF Core migrations.

## Configuration

### SQL Server (Production)
```json
{
  "ConnectionStrings": {
    "ChatDatabase": "Server=(localdb)\\mssqllocaldb;Database=ChatDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "UseInMemoryDatabase": false
}
```

### In-Memory Database (Testing)
```json
{
  "UseInMemoryDatabase": true
}
```

## Usage

### Register services
```csharp
services.AddEntityFrameworkStorage(
    useInMemory: configuration.GetValue<bool>("UseInMemoryDatabase"),
    connectionString: configuration.GetConnectionString("ChatDatabase"));
```

### Ensure database is created
```csharp
await serviceProvider.EnsureDatabaseCreatedAsync();
```

### Apply migrations (production)
```csharp
await serviceProvider.MigrateDatabaseAsync();
```

## Database Schema

### Messages Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| SenderId | uniqueidentifier | User who sent the message |
| Content | nvarchar(4000) | Message content |
| MessageType | int | 0=Public, 1=Private, 2=System |
| TargetUserId | uniqueidentifier | Recipient (null for public messages) |
| Timestamp | datetimeoffset | When the message was sent |

### Indexes
- IX_Messages_SenderId - For queries by sender
- IX_Messages_Timestamp - For chronological ordering
- IX_Messages_TargetUserId_MessageType - For private message queries

## Creating Migrations

```bash
cd Mediator.Chat
dotnet ef migrations add <MigrationName>
```

## Applying Migrations

```bash
dotnet ef database update
```

## Verifying Database

Use the following SQL to verify the database was created correctly:

```sql
-- Check if database exists
SELECT name FROM sys.databases WHERE name = 'ChatDb';

-- Check Messages table structure
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Messages';

-- Check indexes
SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Messages');
```
