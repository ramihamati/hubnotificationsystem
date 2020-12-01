using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Digitteck.HubNotificationSystem
{
    public interface INotificationControllersTable
    {
        void RegisterController<TController, THub, THubActions>()
            where TController : NotificationControllerBase<THub, THubActions>
            where THub : Hub<THubActions>
            where THubActions : class;
        MethodInfo GetMethodInfoForRoute(string routeName);
    }
}