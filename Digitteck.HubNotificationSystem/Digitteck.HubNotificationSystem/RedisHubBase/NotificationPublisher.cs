using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digitteck.HubNotificationSystem
{
    public class NotificationPublisher
    {
        private readonly RedisPublisher _redisPublisher;
        private readonly INotificationRoutesTable _routes;

        public NotificationPublisher(RedisPublisher redisPublisher, INotificationRoutesTable redisControllerRoutes)
        {
            _redisPublisher = redisPublisher;
            _routes = redisControllerRoutes;
        }

        public void Publish<T>(T model, Dictionary<string, string> properties = null)
        {
            //TODO : add a formatter
            RedisExchangeModel exchangeModel = new RedisExchangeModel();
            exchangeModel.Properties = properties ?? new Dictionary<string, string>();

            NotificationRouteDescriptor route = this._routes.GetRoute<T>();

            if (route is null)
            {
                throw new Exception($"THere is no route registered for the model {typeof(T).FullName}");
            }

            exchangeModel.Properties.Add("RouteName", route.RouteName);
            exchangeModel.Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            _redisPublisher.Publish(exchangeModel);
        }

    }
}
