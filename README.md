# Chat Room Application - Mediator Design Pattern Project

## Overview
This project is a **deliberately badly designed** chat room application created to demonstrate the **Mediator Design Pattern** in C# .NET 8. The current implementation has tight coupling between services, making it an ideal candidate for refactoring using the Mediator pattern.

## Project Status: ✅ READY TO REFACTOR

The "bad" implementation is complete and functional. Now you can implement the Mediator pattern to fix the design issues!

## Current Architecture (Badly Designed)

### Problems Demonstrated
1. **Tight Coupling**: `ChatRoomService` directly depends on 5 other services
2. **Too Many Dependencies**: `NotificationService` knows about Email, SMS, Push, and Analytics services
3. **Scattered Logic**: Business rules spread across multiple services
4. **Hard to Test**: Difficult to isolate components for unit testing
5. **Difficult to Extend**: Adding new features requires modifying existing classes

### Current Components

```
ChatRoomService (Orchestrator)
    ├── NotificationService
    │   ├── EmailService
    │   ├── SmsService
    │   ├── PushNotificationService
    │   └── AnalyticsService
    ├── AnalyticsService
    ├── MessageStorageService
    └── UserManagementService
```

### Key Services

- **ChatRoomService**: Main orchestrator with too many responsibilities
- **NotificationService**: Handles all notification types (email, SMS, push)
- **EmailService**: Sends email notifications
- **SmsService**: Sends SMS notifications
- **PushNotificationService**: Sends push notifications
- **AnalyticsService**: Tracks user activity and message statistics
- **MessageStorageService**: Stores and retrieves messages
- **UserManagementService**: Manages user registration and status

## Running the Application

```bash
cd "c:\Users\Garab\source\repos\Mediator"
dotnet run --project Mediators.csproj
```

### Expected Output
The application simulates a chat room with users joining, sending messages, and changing status. You'll see extensive logging showing the tight coupling:

```
=== Chat Room Application Started ===
User Alice joined the chat room
[USER MGMT] User Alice registered
[STORAGE] Message stored
...
User Alice status changed to Online
[PUSH] To User: 1, Message: You are now online
[ANALYTICS] Status change tracked for user 1: Online
...
Message sent by 1: Hello everyone!
[STORAGE] Message stored
[ANALYTICS] Message sent tracked for user 1
[PUSH] To User: 2, Message: Hello everyone!
[ANALYTICS] Message notification tracked for user 2
...
PROBLEMS WITH CURRENT DESIGN:
1. Tight coupling between services
2. Too many dependencies in constructors
3. Hard to test individual components
4. Difficult to add new notification types
5. Business logic scattered across multiple classes

SOLUTION: Implement the Mediator Design Pattern!
```

## Project Structure

```
Mediator/
├── Models/
│   ├── User.cs                    # User model with status
│   └── ChatMessage.cs             # Message model
├── Services/
│   ├── ChatRoomService.cs         # Main service (BADLY DESIGNED)
│   ├── NotificationService.cs     # Notification orchestrator (BADLY DESIGNED)
│   ├── EmailService.cs            # Email sender
│   ├── SmsService.cs              # SMS sender
│   ├── PushNotificationService.cs # Push notification sender
│   ├── AnalyticsService.cs        # Analytics tracker
│   ├── MessageStorageService.cs   # Message storage
│   └── UserManagementService.cs   # User management
├── Program.cs                     # Application entry point
├── Mediators.csproj              # Project file
└── TODO-MEDIATOR-PATTERN.md      # Implementation guide

Mediators.Tests/                   # Unit tests (Note: currently has build issues to fix)
├── Models/
│   ├── UserTests.cs
│   └── ChatMessageTests.cs
├── Services/
│   └── ChatRoomServiceTests.cs
└── Mediators.Tests.csproj
```

## Dependencies

```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
```

### To Add for Mediator Pattern:
```bash
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

## Design Anti-Patterns Demonstrated

### 1. God Object (ChatRoomService)
```csharp
public ChatRoomService(
    ILogger<ChatRoomService> logger,
    NotificationService notificationService,      // BAD
    AnalyticsService analyticsService,            // BAD
    MessageStorageService storageService,         // BAD
    UserManagementService userManagementService)  // BAD
```

### 2. Tight Coupling (NotificationService)
```csharp
public void NotifyUserOfMessage(User user, ChatMessage message)
{
    if (user.Status == UserStatus.Offline)
    {
        _emailService.SendEmail(...);              // Direct coupling
        _smsService.SendSms(...);                  // Direct coupling
    }
    _analyticsService.TrackMessageNotification(...); // Mixed responsibilities
}
```

### 3. Complex Orchestration (SendMessage)
```csharp
public void SendMessage(...)
{
    _messages.Add(message);
    _storageService.StoreMessage(message);         // Direct call
    _analyticsService.TrackMessageSent(...);       // Direct call
    
    foreach (var user in _users.Values)            // Complex loop
    {
        _notificationService.NotifyUserOfMessage(...); // Direct call
    }
    
    _userManagementService.UpdateUserActivity(...); // Direct call
}
```

## How to Implement the Mediator Pattern

See **[TODO-MEDIATOR-PATTERN.md](./TODO-MEDIATOR-PATTERN.md)** for a comprehensive, step-by-step guide including:

- Phase-by-phase implementation plan
- Request and Notification definitions
- Handler implementations
- Pipeline behaviors for cross-cutting concerns
- Testing strategies
- Expected benefits

## Learning Objectives

By implementing the Mediator pattern in this project, you will learn:

1. ✅ How to identify tight coupling in existing code
2. ✅ How to design a mediator interface
3. ✅ Request/Response pattern (Commands/Queries)
4. ✅ Notification pattern (Events)
5. ✅ Pipeline behaviors for cross-cutting concerns
6. ✅ Dependency Injection with mediators
7. ✅ Testing strategies for mediator-based architectures
8. ✅ CQRS basics (Command Query Responsibility Segregation)

## Comparison: Before vs After Mediator Pattern

### Before (Current)
```csharp
// ChatRoomService has 5 dependencies
public ChatRoomService(
    ILogger logger,
    NotificationService notifications,
    AnalyticsService analytics,
    MessageStorageService storage,
    UserManagementService userManagement) { }

// Direct coupling
public void SendMessage(...)
{
    _storage.StoreMessage(message);
    _analytics.TrackMessage(...);
    _notifications.NotifyUsers(...);
    _userManagement.UpdateActivity(...);
}
```

### After (With Mediator)
```csharp
// ChatRoomService has 1 dependency
public ChatRoomService(IMediator mediator) { }

// Decoupled through mediator
public async Task SendMessage(...)
{
    var response = await _mediator.Send(new SendMessageRequest(...));
    await _mediator.Publish(new MessageSentNotification(...));
}
```

## Benefits of Mediator Pattern

### ✅ Loose Coupling
- Components don't know about each other
- Easy to add/remove/replace components
- Changes isolated to specific handlers

### ✅ Single Responsibility
- Each handler does one thing
- Clear separation of concerns
- Focused, maintainable code

### ✅ Testability
- Easy to test handlers in isolation
- Simple mocking
- Independent unit tests

### ✅ Extensibility
- Add new handlers without modifying existing code
- Add new notification channels easily
- Open/Closed Principle

### ✅ Cross-Cutting Concerns
- Logging, validation, caching via pipeline behaviors
- Applied automatically to all requests
- No code duplication

## Key Concepts

### Request/Response (IRequest<TResponse>)
Used for commands and queries that return a result:
```csharp
public record SendMessageRequest(...) : IRequest<SendMessageResponse>;
```

### Notifications (INotification)
Used for events that multiple handlers can respond to:
```csharp
public record MessageSentNotification(...) : INotification;
```

### Handlers
Process requests and notifications:
```csharp
public class SendMessageHandler : IRequestHandler<SendMessageRequest, SendMessageResponse>
{
    public async Task<SendMessageResponse> Handle(SendMessageRequest request, CancellationToken ct)
    {
        // Process request
    }
}
```

### Pipeline Behaviors
Cross-cutting concerns:
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // Before
        var response = await next();
        // After
        return response;
    }
}
```

## Resources

- [Microsoft Learn - Mediator Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api)
- [MediatR GitHub Repository](https://github.com/jbogard/MediatR)
- [CQRS Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)

## Next Steps

1. ✅ Run the current application to see the problems
2. ✅ Review TODO-MEDIATOR-PATTERN.md for implementation guide
3. ⬜ Install MediatR packages
4. ⬜ Start with Phase 2: Design the Mediator Infrastructure
5. ⬜ Implement requests and notifications
6. ⬜ Create handlers
7. ⬜ Add pipeline behaviors
8. ⬜ Update tests
9. ⬜ Compare before/after

**Happy refactoring! 🚀**
