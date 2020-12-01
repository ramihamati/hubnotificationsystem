using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Digitteck.HubNotificationSystem
{
    public class NotificationControllersTable : INotificationControllersTable
    {
        internal ConcurrentBag<Type> _controllers = new ConcurrentBag<Type>();

        public void RegisterController<TController, THub, THubActions>()
            where TController : NotificationControllerBase<THub, THubActions>
            where THub : Hub<THubActions>
            where THubActions : class
        {
            _controllers.Add(typeof(TController));
        }

        public MethodInfo GetMethodInfoForRoute(string routeName)
        {
            foreach (var controllerType in _controllers)
            {
                foreach (MethodInfo method in controllerType.GetMethods())
                {
                    if (method.GetCustomAttribute<NotificationRouteAttribute>() is NotificationRouteAttribute attr)
                    {
                        if (attr.RouteName == routeName)
                        {
                            return method;
                        }
                    }
                }
            }

            return null;
        }
    }
}
