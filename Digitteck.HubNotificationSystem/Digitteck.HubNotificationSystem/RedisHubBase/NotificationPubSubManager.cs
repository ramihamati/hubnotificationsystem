using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Digitteck.HubNotificationSystem
{
    public sealed class NotificationPubSubManager<THub, THubActions>
        where THub : Hub<THubActions>
        where THubActions : class
    {
        private readonly RedisManager _redisManager;
        private readonly NotificationKeyBuilder _keyBuilder;
        private readonly INotificationRoutesTable _routes;
        private readonly INotificationControllersTable _controllerRegistry;
        private readonly IServiceProvider _serviceProvider;

        public static ConcurrentDictionary<string, NotificationSubscriber> HubSubscribers
          = new ConcurrentDictionary<string, NotificationSubscriber>();

        public NotificationPubSubManager(
            IServiceProvider serviceProvider,
            NotificationKeyBuilder keyBuilder,
            RedisManager redisManager,
            NotificationOptions notificationOptions)
        {
            _redisManager = redisManager;
            _routes = notificationOptions.ControllerRoutes;
            _controllerRegistry = notificationOptions.ControllerRegistry;
            _keyBuilder = keyBuilder;
            _serviceProvider = serviceProvider;
        }

        public NotificationPublisher GetPublisher(string userId)
        {
            string channelName = this._keyBuilder.BuildChannelName(userId);
            return new NotificationPublisher(_redisManager.GetPublisher(channelName), this._routes);
        }

        public void Subscribe(string connectionId, string userId)
        {
            string channelName = this._keyBuilder.BuildChannelName(userId);
            string subscriberKey = this._keyBuilder.BuildSubscriberKey(connectionId, userId);

            RedisSubscriber redisSub = _redisManager.GetSubscriber(channelName);
            NotificationSubscriber hubSub = new NotificationSubscriber(redisSub, connectionId, userId);

            if (HubSubscribers.TryAdd(subscriberKey, hubSub))
            {
                hubSub.OnMessage(async message =>
                {
                    // check exchange message integrity
                    if (message.Properties?.ContainsKey("RouteName") != true)
                    {
                        throw new Exception("The exchange message does not contain a route name property");
                    }

                    string routeName = message.Properties["RouteName"];

                    // check registered routes
                    NotificationRouteDescriptor route = this._routes.GetRoute(routeName);

                    if (route is null)
                    {
                        throw new Exception($"Could not find route with name {route.RouteName}. Each message must contain a route name that will redirect the message handling to the proper controller");
                    }

                    MethodInfo methodInfo = this._controllerRegistry.GetMethodInfoForRoute(route.RouteName);

                    if (methodInfo is null)
                    {
                        throw new Exception("Could not found a controller that contains a method decorated with " +
                            $"{nameof(NotificationRouteAttribute)} to handle the received message");
                    }

                    // create controller
                    object controllerObject = ActivatorUtilities.CreateInstance(this._serviceProvider, methodInfo.ReflectedType);
                    FieldInfo subscriberInfo = FindField(methodInfo.ReflectedType, "_subscriber");

                    if (subscriberInfo is null)
                    {
                        throw new ApplicationException("Could not find _subscriber field in the redis controller base class");
                    }

                    // initialize controller properties
                    subscriberInfo.SetValue(controllerObject, hubSub);

                    // deserialize exchange model inner model
                    object model = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), route.ModelType);

                    // invoke controller method
                    object result = methodInfo.Invoke(controllerObject, new object[] { model });

                    if (typeof(Task).IsAssignableFrom(result.GetType()))
                    {
                        Task taskResult = result as Task;
                        await taskResult;
                    }
                });
                //this.OnNewSubscriber(hubSub);
            }
        }

        public void Unsubscribe(string connectionId, string userId)
        {
            string subscriberKey = this._keyBuilder.BuildSubscriberKey(connectionId, userId);
            if (HubSubscribers.TryRemove(subscriberKey, out NotificationSubscriber hub))
            {
                hub.Unsubscribe();
            }
        }

        private FieldInfo FindField(Type type,string fieldName)
        {
            FieldInfo subscriberInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (subscriberInfo is null && type.BaseType is not null)
            {
                return FindField(type.BaseType, fieldName);
            }

            return subscriberInfo;
        }
    }
}
