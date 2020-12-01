using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Digitteck.HubNotificationSystem
{
    /// <summary>
    /// A hub where the caller can register in a subscribtion and the worker will receive messages on his channel 
    /// and send him notifications
    /// </summary>
    /// <typeparam name="THub">The actual Hub class implementing <see cref="Hub{T}"/> where T is <typeparamref name="THubActions"/></typeparam>
    /// <typeparam name="THubActions">This interface contains end points definitions for the hub</typeparam>
    [Authorize]
    public class NotificationHub<THub, THubActions> : Hub<THubActions>
        where THub : Hub<THubActions>
        where THubActions : class
    {
        private readonly NotificationPubSubManager<THub, THubActions> _redisManager;

        public NotificationHub(INotificationPubSubProvider provider)
        {
            _redisManager = ((NotificationPubSubProvider)provider).GetManager<THub, THubActions>();
        }

        public void Subscribe()
        {
            string userIdentifier = this.Context.UserIdentifier;
            _redisManager.Subscribe(this.Context.ConnectionId, userIdentifier);
        }

        public void Unubscribe()
        {
            string userIdentifier = this.Context.UserIdentifier;
            _redisManager.Unsubscribe(this.Context.ConnectionId, userIdentifier);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userIdentifier = this.Context.UserIdentifier;
            this._redisManager.Unsubscribe(this.Context.ConnectionId, userIdentifier);
            return Task.CompletedTask;
        }
    }
}
