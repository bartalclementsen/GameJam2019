using System;

namespace Core.Mediators
{
    public interface IMessenger
    {
        ISubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : IMessage;

        void Publish(IMessage message);

        void Unsubscribe(ISubscriptionToken subscriptionToken);
    }
}
