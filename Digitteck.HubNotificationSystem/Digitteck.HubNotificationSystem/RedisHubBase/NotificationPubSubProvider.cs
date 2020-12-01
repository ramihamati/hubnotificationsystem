using Digitteck.HubNotificationSystem;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class NotificationPubSubProvider : INotificationPubSubProvider
    {
        private Dictionary<Type, object> subscribersManagers = new Dictionary<Type, object>();
        private IServiceProvider ServiceProvider { get; }

        public NotificationPubSubProvider(IServiceProvider sp)
        {
            ServiceProvider = sp;
        }

        public void AddManager<THub, THubActions>(Action<NotificationOptions> settings)
            where THub : Hub<THubActions>
            where THubActions : class
        {
            if (!subscribersManagers.ContainsKey(typeof(THubActions)))
            {
                NotificationOptions options = new NotificationOptions(new NotificationRoutesTable(), new NotificationControllersTable());
                settings(options);

                if (options.KeyBuilderBuilderType is null)
                {
                    throw new Exception("You must define a key builder in the extension settings");
                }
                NotificationKeyBuilder keyBuilder = (NotificationKeyBuilder)ActivatorUtilities.CreateInstance(ServiceProvider, options.KeyBuilderBuilderType);

                NotificationEvents notificationEvents = new NoopNotificationEvents();
                if (!(options.NotificationEventsType is null))
                {
                    notificationEvents = (NotificationEvents)ActivatorUtilities.CreateInstance(ServiceProvider, options.NotificationEventsType);
                }

                RedisConnection connection = ServiceProvider.GetRequiredService<IRedisSettingsProvider>().GetConnectionSettings();
                RedisConnectionManager connectionManager = new RedisConnectionManager(connection, notificationEvents);
                RedisManager redisManager = new RedisManager(connectionManager, notificationEvents);

                NotificationPubSubManager<THub, THubActions> subManager = new NotificationPubSubManager<THub, THubActions>(ServiceProvider, keyBuilder, redisManager, options);

                subscribersManagers.Add(typeof(THubActions), subManager);
            }
            // do not throw exception. This method must be idemponent (no gains in throwing an exception)
        }

        public NotificationPubSubManager<THub, THubActions> GetManager<THub, THubActions>()
            where THub : Hub<THubActions>
            where THubActions : class
        {
            if (subscribersManagers.ContainsKey(typeof(THubActions)))
            {
                return (NotificationPubSubManager<THub, THubActions>)subscribersManagers[typeof(THubActions)];
            }

            return null;
        }
    }
}
