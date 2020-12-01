using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Digitteck.HubNotificationSystem
{
    public class RedisPublisher
    {
        private readonly ISubscriber _subscriber;
        private readonly string _channel;
        private readonly NotificationEvents _nEvents;

        public RedisPublisher(ISubscriber subscriber, string channel, NotificationEvents nEvents)
        {
            _subscriber = subscriber;
            _channel = channel;
            _nEvents = nEvents;
        }

        public void Publish(RedisExchangeModel redisExchangeModel)
        {
            _subscriber.Publish(_channel, JsonConvert.SerializeObject(redisExchangeModel));
        }

        public Task PublishAsync(RedisExchangeModel redisExchangeModel)
        {
            return _subscriber.PublishAsync(_channel, JsonConvert.SerializeObject(redisExchangeModel));
        }

        public RedisSubscriber GetSubscriber()
        {
            return new RedisSubscriber(_subscriber, _channel, _nEvents);
        }
    }
}
