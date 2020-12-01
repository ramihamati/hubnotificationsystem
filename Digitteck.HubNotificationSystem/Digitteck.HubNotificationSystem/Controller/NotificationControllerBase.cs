using Microsoft.AspNetCore.SignalR;

namespace Digitteck.HubNotificationSystem
{
    public abstract class NotificationControllerBase<THub, THubActions>
        where THub : Hub<THubActions>
        where THubActions : class
    {
#pragma warning disable RCS1169 // Make field read-only.
#pragma warning disable IDE0044 // Add readonly modifier
        // do not use auto-property - it's reflection teritory
        private NotificationSubscriber _subscriber;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore RCS1169 // Make field read-only.
        public NotificationSubscriber Subscriber => _subscriber;

        public THubActions GetSubsriberHub(IHubContext<THub, THubActions> hubContext)
        {
            return hubContext.Clients.Client(this._subscriber.ConnectionId);
        }

    }
}
