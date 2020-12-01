using System;

namespace Digitteck.HubNotificationSystem
{
    public sealed class NotificationPublisherClient
    {
        private readonly RedisManager _redisManager;
        private readonly NotificationKeyBuilder _keyBuilder;
        private readonly INotificationRoutesTable _routes;

        public NotificationPublisherClient(
            NotificationKeyBuilder keyBuilder,
            RedisManager redisManager,
            NotificationPublisherOptions options)
        {

            this._keyBuilder = keyBuilder;
            _redisManager = redisManager;
            _routes = options.RedisControllerRoutes;
        }

        public NotificationPublisher GetPublisherFor(string userId)
        {
            string channelName = this._keyBuilder.BuildChannelName(userId);
            return new NotificationPublisher(_redisManager.GetPublisher(channelName), this._routes);
        }
    }
}
