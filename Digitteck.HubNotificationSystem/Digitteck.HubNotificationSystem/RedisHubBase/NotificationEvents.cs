using Microsoft.AspNetCore.SignalR;
using System;

namespace Digitteck.HubNotificationSystem
{
    internal class NoopNotificationEvents : NotificationEvents
    {
        public override void OnException(Exception exception)
        {
        }

        public override void OnInformation(string information)
        {
        }
    }

    public abstract class NotificationEvents
    {
        public abstract void OnException(Exception exception);

        public abstract void OnInformation(string information);
    }
}
