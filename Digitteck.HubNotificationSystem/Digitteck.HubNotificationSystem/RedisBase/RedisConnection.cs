namespace Digitteck.HubNotificationSystem { 
    public class RedisConnection
    {
        public string Host { get; }
        public int Port { get; }
        public RedisConnection(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
