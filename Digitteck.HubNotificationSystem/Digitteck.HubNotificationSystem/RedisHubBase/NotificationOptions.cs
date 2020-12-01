using Microsoft.AspNetCore.SignalR;
using System;

namespace Digitteck.HubNotificationSystem
{
    public class NotificationOptions
    {
        internal INotificationRoutesTable ControllerRoutes { get; }
        internal INotificationControllersTable ControllerRegistry { get; }

        internal Type KeyBuilderBuilderType;

        internal Type NotificationEventsType;

        public NotificationOptions(
            INotificationRoutesTable redisControllerRoutes,
            INotificationControllersTable notificationControllerRegistry)
        {
            ControllerRoutes = redisControllerRoutes;
            ControllerRegistry = notificationControllerRegistry;
        }

        public void RegisterRoute<TModel>(string route)
        {
            ControllerRoutes.RegisterRoute<TModel>(route);
        }

        public void RegisterController<TController, THub, THubActions>()
           where TController : NotificationControllerBase<THub, THubActions>
           where THub : Hub<THubActions>
           where THubActions : class
        {
            ControllerRegistry.RegisterController<TController, THub, THubActions>();
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
