using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Digitteck.HubNotificationSystem
{
    public class RedisSubscriber
    {
        public readonly ISubscriber _subscriber;
        private readonly string _channel;
        private readonly ChannelMessageQueue _channelMessageQueue;
        private readonly NotificationEvents _nEvents;

        public RedisSubscriber(
            ISubscriber subscriber,
            string channel,
            NotificationEvents nEvents)
        {
            _subscriber = subscriber;
            _channel = channel;
            _channelMessageQueue = subscriber.Subscribe(channel);
            _nEvents = nEvents;
        }

        public void Unsubscribe()
        {
            _channelMessageQueue.Unsubscribe();
            //this._subscriber?.Unsubscribe(this._channel); ?? this removes subscriptios for all subscribers. Must investigate
        }

        public void OnMessage(Action<RedisExchangeModel> handler)
        {
            _channelMessageQueue.OnMessage(channelMessage =>
            {
                try
                {
                    var message = channelMessage.Message;
                    RedisExchangeModel exchangeModel = JsonConvert.DeserializeObject<RedisExchangeModel>(message);
                    handler(exchangeModel);
                }
                catch (Exception ex) when (!(_nEvents is null))
                {
                    _nEvents.OnException(ex);
                }
            });
        }

        public void OnMessage(Func<RedisExchangeModel, Task> asyncHandler)
        {
            _channelMessageQueue.OnMessage(async channelMessage =>
            {
                try
                {
                    var message = channelMessage.Message;
                    RedisExchangeModel exchangeModel = JsonConvert.DeserializeObject<RedisExchangeModel>(message);
                    await asyncHandler(exchangeModel);
                }
                catch (Exception ex) when (!(_nEvents is null))
                {
                    _nEvents.OnException(ex);
                }
            });
        }
    }
}
