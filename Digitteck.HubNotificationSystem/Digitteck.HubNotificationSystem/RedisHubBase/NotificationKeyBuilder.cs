namespace Digitteck.HubNotificationSystem
{
    public abstract class NotificationKeyBuilder
    {
        public abstract string BuildChannelName(string userId);
        public abstract string BuildSubscriberKey(string connectionId, string userId);
    }
}
