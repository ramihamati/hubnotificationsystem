using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Digitteck.HubNotificationSystem
{
    public class RedisConnectionManager
    {
        private readonly RedisConnection _redisConnection;
        private ConfigurationOptions _configurationOptions;
        private readonly NotificationEvents _nEvents;

        public ConnectionMultiplexer Connection { get; private set; }
        public RedisConnectionManager(RedisConnection redisConnection, NotificationEvents nEvents)
        {
            _redisConnection = redisConnection;
            _configurationOptions = new ConfigurationOptions();
            //_configurationOptions.User  //TOOD: implement user/password/ssl
            //_configurationOptions.TrustIssuer(new X509Certificate2)
            _configurationOptions.EndPoints.Add(_redisConnection.Host, _redisConnection.Port);
            _nEvents = nEvents;
        }

        public IDatabase GetDatabase()
        {
            if (this.Connection is null)
            {
                this.Connect();
            }

            return this.Connection.GetDatabase();
        }

        public ISubscriber GetSubscriber()
        {
            if (this.Connection is null)
            {
                this.Connect();
            }

            return this.Connection.GetSubscriber();
        }

        private void Connect()
        {
            this.Connection = ConnectionMultiplexer.Connect(this._configurationOptions);

            this.Connection.ConnectionFailed += Connection_ConnectionFailed;
            this.Connection.ConnectionRestored += Connection_ConnectionRestored;
        }

        private void Connection_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            if (_nEvents is null)
            {
                throw e.Exception;
            }
            else
            {
                _nEvents.OnInformation($"Connection restored on {e.EndPoint}.");
            }
        }

        private void Connection_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            if (_nEvents is null)
            {
                throw e.Exception;
            }
            else
            {
                _nEvents.OnException(e.Exception);
            }
        }
    }
}
