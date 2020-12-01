using Digitteck.HubNotificationSystem;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface INotificationPublisherFactoryBuilder
    {
        void AddManager(string publisherName, Action<NotificationPublisherOptions> settings);
    }
}
