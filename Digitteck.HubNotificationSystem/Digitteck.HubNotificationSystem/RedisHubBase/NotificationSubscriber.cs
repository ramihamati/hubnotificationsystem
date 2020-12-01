using System;
using System.Threading.Tasks;

namespace Digitteck.HubNotificationSystem
{

    public class NotificationSubscriber
    {
        public string ConnectionId { get; }
        public string UserId { get; }

        private readonly RedisSubscriber _redisSubscriber;

        public NotificationSubscriber(RedisSubscriber redisSubscriber, string connectionId, string userId)
        {
            _redisSubscriber = redisSubscriber;
            ConnectionId = connectionId;
            UserId = userId;
        }

        public void OnMessage(Action<RedisExchangeModel> handler)
        {
            _redisSubscriber.OnMessage(handler);
        }

        public void OnMessage(Func<RedisExchangeModel, Task> asyncHandler)
        {
            _redisSubscriber.OnMessage(asyncHandler);
        }

        internal void Unsubscribe()
        {
            _redisSubscriber.Unsubscribe();
        }
    }
}
