using System;

namespace Digitteck.HubNotificationSystem
{
    public class NotificationPublisherOptions
    {
        internal INotificationRoutesTable RedisControllerRoutes { get; }
        internal Type KeyBuilderBuilderType;
        internal Type NotificationEventsType;
        public NotificationPublisherOptions(INotificationRoutesTable redisControllerRoutes)
        {
            RedisControllerRoutes = redisControllerRoutes;
        }

        public void RegisterRoute<TModel>(string route)
        {
            RedisControllerRoutes.RegisterRoute<TModel>(route);
        }

        public void UseKeyBuilder<TKeyBuilder>()
          where TKeyBuilder : NotificationKeyBuilder
        {
            KeyBuilderBuilderType = typeof(TKeyBuilder);
        }

        public void UseNotificationEvents<TNotificationEvents>() where TNotificationEvents : NotificationEvents
        {
            this.NotificationEventsType = typeof(TNotificationEvents);
        }
    }
}
