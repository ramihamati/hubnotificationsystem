using System;

namespace Digitteck.HubNotificationSystem
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NotificationRouteAttribute : Attribute
    {
        public NotificationRouteAttribute(string routeName)
        {
            RouteName = routeName;
        }

        public string RouteName { get; }
    }
}
