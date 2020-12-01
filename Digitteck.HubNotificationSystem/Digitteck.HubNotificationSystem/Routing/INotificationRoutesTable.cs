namespace Digitteck.HubNotificationSystem
{
    public interface INotificationRoutesTable
    {
        NotificationRouteDescriptor GetRoute(string routeName);
        NotificationRouteDescriptor GetRoute<TModel>();
        void RegisterRoute<TModel>(string route);
    }
}