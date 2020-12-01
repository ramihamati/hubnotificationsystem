using Digitteck.HubNotificationSystem;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface INotificationPubSubProvider
    {
        public void AddManager<THub, THubActions>(Action<NotificationOptions> settings)
             where THub : Hub<THubActions>
             where THubActions : class;
    }
}
