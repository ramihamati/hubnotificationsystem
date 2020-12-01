using Digitteck.HubNotificationSystem;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class NotificationPublisherFactory : INotificationPublisherFactory, INotificationPublisherFactoryBuilder
    {
        private Dictionary<string, Func<NotificationPublisherClient>> publisherManagers = new Dictionary<string, Func<NotificationPublisherClient>>();

        private IServiceProvider ServiceProvider { get; }

        public NotificationPublisherFactory(IServiceProvider sp)
        {
            ServiceProvider = sp;
        }

        public void AddManager(string publisherName, Action<NotificationPublisherOptions> settings)
        {
            if (this.publisherManagers.ContainsKey(publisherName))
            {
                return;
            }

            publisherManagers.Add(publisherName, () =>
            {
                NotificationPublisherOptions options = new NotificationPublisherOptions(new NotificationRoutesTable());
                settings(options);
                RedisConnection connection = ServiceProvider.GetRequiredService<IRedisSettingsProvider>().GetConnectionSettings();

                NotificationEvents notificationEvents = new NoopNotificationEvents();

                if (!(options.NotificationEventsType is null))
                {
                    notificationEvents = (NotificationEvents)ActivatorUtilities.CreateInstance(ServiceProvider, options.NotificationEventsType);
                }

                RedisConnectionManager connectionManager = new RedisConnectionManager(connection, notificationEvents);
                RedisManager redisManager = new RedisManager(connectionManager, notificationEvents);

                if (options.KeyBuilderBuilderType is null)
                {
                    throw new Exception("You must define a key builder in the extension settings");
                }

                NotificationKeyBuilder keyBuilder = (NotificationKeyBuilder)ActivatorUtilities.CreateInstance(ServiceProvider, options.KeyBuilderBuilderType);

                return new NotificationPublisherClient(keyBuilder, redisManager, options);
            });
        }

        public NotificationPublisherClient GetPublisherClient(string publisherName)
        {
            if (publisherManagers.ContainsKey(publisherName))
            {
                return publisherManagers[publisherName]();
            }

            return null;
        }
    }
}
