using Digitteck.HubNotificationSystem;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface INotificationPublisherFactory
    {
        NotificationPublisherClient GetPublisherClient(string publisherName);
    }
}