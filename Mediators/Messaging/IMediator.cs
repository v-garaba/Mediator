using Mediators.Messaging.Notifications;
using Mediators.Messaging.Requests;

namespace Mediators.Messaging;

public interface IMediator : INotificationObserver, IRequestsObserver { }
