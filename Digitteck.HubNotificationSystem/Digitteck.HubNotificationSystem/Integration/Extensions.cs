using Digitteck.HubNotificationSystem;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisExtensions
    {
        /// <summary>
        /// ads a publisher only for redis
        /// </summary>
        /// <typeparam name="TSettingsProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="keyBuilder"></param>
        public static void AddNotificationPublisherProvider<TSettingsProvider>(this IServiceCollection services, Action<INotificationPublisherFactoryBuilder> settings)
            where TSettingsProvider : class, IRedisSettingsProvider
        {
            services.TryAddSingleton<IRedisSettingsProvider, TSettingsProvider>();

            services.AddSingleton<INotificationPublisherFactory>(sp =>
            {
                NotificationPublisherFactory factory = new NotificationPublisherFactory(sp);
                settings(factory);
                return factory;
            });
        }

        public static void AddRedisManager<TSettingsProvider>(this IServiceCollection services, Action<INotificationPubSubProvider> builder)
            where TSettingsProvider : class, IRedisSettingsProvider
        {
            services.TryAddSingleton<IRedisSettingsProvider, TSettingsProvider>();

            services.AddSingleton<INotificationPubSubProvider>(sp =>
            {
                INotificationPubSubProvider subscriberManagerProvider = new NotificationPubSubProvider(sp);
                builder(subscriberManagerProvider);
                return subscriberManagerProvider;
            });
        }
    }
}
