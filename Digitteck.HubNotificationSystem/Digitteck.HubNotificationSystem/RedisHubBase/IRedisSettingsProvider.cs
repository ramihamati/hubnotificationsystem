namespace Digitteck.HubNotificationSystem
{
    public interface IRedisSettingsProvider
    {
        RedisConnection GetConnectionSettings();
    }
}
