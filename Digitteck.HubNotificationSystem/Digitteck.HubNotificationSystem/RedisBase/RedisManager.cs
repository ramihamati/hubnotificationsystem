using Microsoft.Extensions.Logging;

namespace Digitteck.HubNotificationSystem
{
    public class RedisManager
    {
        private readonly RedisConnectionManager _redisConnectionManager;
        private readonly NotificationEvents _nEvents;

        public RedisManager(RedisConnectionManager redisConnectionManager, NotificationEvents nEvents)
        {
            _redisConnectionManager = redisConnectionManager;
            _nEvents = nEvents;
        }

        public RedisSubscriber GetSubscriber(string channel)
        {
            return new RedisSubscriber(_redisConnectionManager.GetSubscriber(), channel, _nEvents);
        }

        public RedisPublisher GetPublisher(string channel)
        {
            return new RedisPublisher(_redisConnectionManager.GetSubscriber(), channel, _nEvents);
        }
    }
}
