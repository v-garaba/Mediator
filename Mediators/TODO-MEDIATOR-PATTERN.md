# Mediator Pattern - Implementation Guide

## Current Problems
- **Tight Coupling**: Services directly call each other
- **Too Many Dependencies**: 5+ dependencies in constructors
- **Hard to Test**: Difficult to isolate components
- **Hard to Extend**: Changes require modifying multiple classes

---

## Implementation Plan

### Phase 2: Mediator Infrastructure
- [ ] **Create IMediator Interface**
  - Define `Send<TRequest, TResponse>` method for requests
  - Define `Publish<TNotification>` method for notifications/events
  
- [ ] **Create Base Classes**
  - `IRequest<TResponse>` - Interface for requests that expect a response
  - `INotification` - Interface for events that don't expect a response
  - `IRequestHandler<TRequest, TResponse>` - Interface for request handlers
  - `INotificationHandler<TNotification>` - Interface for event handlers

- [ ] **Implement Concrete Mediator**
  - Create `ChatMediator` class implementing `IMediator`
  - Set up handler registration and discovery
  - Implement message routing logic

### Phase 3: Define Requests and Notifications (Events)

#### **Requests** (Commands/Queries that expect responses)
- [ ] `SendMessageRequest`
  ```csharp
  public record SendMessageRequest(string SenderId, string Content, MessageType Type, string? TargetUserId = null) : IRequest<SendMessageResponse>;
  ```
  
- [ ] `AddUserRequest`
  ```csharp
  public record AddUserRequest(User User) : IRequest<AddUserResponse>;
  ```
  
- [ ] `ChangeUserStatusRequest`
  ```csharp
  public record ChangeUserStatusRequest(string UserId, UserStatus NewStatus) : IRequest<bool>;
  ```
  
- [ ] `GetMessagesQuery`
  ```csharp
  public record GetMessagesQuery() : IRequest<IReadOnlyList<ChatMessage>>;
  ```

#### **Notifications** (Events for broadcasting)
- [ ] `MessageSentNotification`
  ```csharp
  public record MessageSentNotification(ChatMessage Message) : INotification;
  ```
  
- [ ] `UserJoinedNotification`
  ```csharp
  public record UserJoinedNotification(User User) : INotification;
  ```
  
- [ ] `UserStatusChangedNotification`
  ```csharp
  public record UserStatusChangedNotification(string UserId, UserStatus OldStatus, UserStatus NewStatus) : INotification;
  ```

### Phase 4: Implement Request Handlers

- [ ] **SendMessageHandler** : `IRequestHandler<SendMessageRequest, SendMessageResponse>`
  - Store message
  - Return response
  - Publish `MessageSentNotification`

- [ ] **AddUserHandler** : `IRequestHandler<AddUserRequest, AddUserResponse>`
  - Register user
  - Publish `UserJoinedNotification`

- [ ] **ChangeUserStatusHandler** : `IRequestHandler<ChangeUserStatusRequest, bool>`
  - Update user status
  - Publish `UserStatusChangedNotification`

- [ ] **GetMessagesQueryHandler** : `IRequestHandler<GetMessagesQuery, IReadOnlyList<ChatMessage>>`
  - Query and return messages

### Phase 5: Implement Notification Handlers

- [ ] **MessageSentNotificationHandler** : `INotificationHandler<MessageSentNotification>`
  - Track analytics
  - Trigger user activity update

- [ ] **UserNotificationHandler** : `INotificationHandler<MessageSentNotification>`
  - Notify target users (email/SMS/push based on status)
  - Handle private vs public message notifications

- [ ] **UserJoinedNotificationHandler** : `INotificationHandler<UserJoinedNotification>`
  - Send system message
  - Log user join event

- [ ] **UserStatusChangedNotificationHandler** : `INotificationHandler<UserStatusChangedNotification>`
  - Notify user of status change
  - Track analytics
  - Update user management

### Phase 6: Refactor Services

- [ ] **Refactor ChatRoomService**
  - Remove direct dependencies on other services
  - Inject only `IMediator`
  - Replace direct service calls with mediator.Send() or mediator.Publish()

- [ ] **Refactor NotificationService**
  - Keep email/SMS/push services (low-level operations)
  - Remove business logic
  - Make it respond to notifications only

- [ ] **Simplify AnalyticsService**
  - Subscribe to events via notification handlers
  - Remove direct calls from other services

- [ ] **Simplify UserManagementService**
  - Subscribe to events via notification handlers
  - Focus only on user data management

### Phase 7: Add Cross-Cutting Concerns (Pipeline Behaviors)

- [ ] **Create LoggingBehavior** : `IPipelineBehavior<TRequest, TResponse>`
  ```csharp
  public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
      // Log before and after each request
  }
  ```

- [ ] **Create ValidationBehavior** : `IPipelineBehavior<TRequest, TResponse>`
  ```csharp
  public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
      // Validate requests before processing
  }
  ```

- [ ] **Create PerformanceBehavior** : `IPipelineBehavior<TRequest, TResponse>`
  ```csharp
  public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  {
      // Measure and log execution time
  }
  ```

### Phase 8: Update Dependency Injection

- [ ] Register mediator in DI container
- [ ] Register all request handlers
- [ ] Register all notification handlers  
- [ ] Register pipeline behaviors
- [ ] Remove unnecessary service dependencies

### Phase 9: Update Tests

- [ ] **Unit Test Request Handlers**
  - Test handlers in isolation
  - Mock only necessary dependencies

- [ ] **Unit Test Notification Handlers**
  - Test each handler independently
  - Verify correct behavior for each event

- [ ] **Integration Tests**
  - Test complete request/notification flow
  - Verify multiple handlers respond to same notification

- [ ] **Test Pipeline Behaviors**
  - Verify logging behavior
  - Verify validation behavior
  - Verify performance tracking

### Phase 10: Documentation and Comparison

- [ ] **Create "Before" Documentation**
  - Document current tight coupling
  - Show dependency graphs

- [ ] **Create "After" Documentation**
  - Document mediator pattern implementation
  - Show new loosely coupled architecture

- [ ] **Performance Comparison**
  - Measure performance before/after
  - Document any overhead from mediator pattern

- [ ] **Benefits Documentation**
  - List testability improvements
  - List extensibility improvements
  - List maintainability improvements

---

## Key Concepts

**Request/Response**:
```csharp
var response = await _mediator.Send(new SendMessageRequest(...));
// One handler, returns response
```

**Notification** (Event):
```csharp
await _mediator.Publish(new MessageSentNotification(...));
// Multiple handlers, no response
```

**Pipeline Behavior** (Cross-cutting):
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.LogInformation($"Handling {typeof(TRequest).Name}");
        var response = await next();
        _logger.LogInformation($"Handled");
        return response;
    }
}
```

## Benefits
- **Decoupling**: Services communicate via mediator only
- **Single Responsibility**: One handler = one job
- **Testability**: Easy to mock and test in isolation
- **Extensibility**: Add handlers without changing existing code

## Resources
- [Microsoft Docs - Mediator Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api)
