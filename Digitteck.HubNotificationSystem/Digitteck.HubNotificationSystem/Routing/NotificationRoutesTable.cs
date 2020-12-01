using System;
using System.Collections.Generic;

namespace Digitteck.HubNotificationSystem
{
    public class NotificationRoutesTable : INotificationRoutesTable
    {
        private List<NotificationRouteDescriptor> Routes { get; set; }

        public NotificationRoutesTable()
        {
            Routes = new List<NotificationRouteDescriptor>();
        }

        public void RegisterRoute<TModel>(string route)
        {
            var existing = this.Routes.Find(x => x.RouteName == route);
            if (!(existing is null))
            {
                throw new Exception($"Cannot register a route with the same name \'{route}\' twice");
            }

            Type modelType = typeof(TModel);
            existing = this.Routes.Find(x => x.ModelType.Equals(modelType));
            if (!(existing is null))
            {
                throw new Exception($"Cannot register a route for the same model  \'{modelType.FullName}\' twice");
            }

            this.Routes.Add(new NotificationRouteDescriptor
            {
                ModelType = typeof(TModel),
                RouteName = route
            });
        }

        public NotificationRouteDescriptor GetRoute(string routeName)
        {
            return this.Routes.Find(x => x.RouteName == routeName);
        }

        public NotificationRouteDescriptor GetRoute<TModel>()
        {
            Type modelType = typeof(TModel);
            return this.Routes.Find(x => x.ModelType.Equals(modelType));
        }
    }
}
