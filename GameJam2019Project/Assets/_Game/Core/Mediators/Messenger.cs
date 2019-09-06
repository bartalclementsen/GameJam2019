using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Mediators
{
    public class Messenger : IMessenger
    {
        private readonly IDictionary<Type, IDictionary<Guid, ISubscription>> _subscriptions = new Dictionary<Type, IDictionary<Guid, ISubscription>>();
        private readonly object _theLock = new object();

        /* ----------------------------------------------------------------------------  */
        /*                                PUBLIC METHODS                                 */
        /* ----------------------------------------------------------------------------  */
        public void Publish(IMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Type messageType = message.GetType();

            List<ISubscription> toNotify = null;

            lock (_theLock)
            {
                IDictionary<Guid, ISubscription> messageSubscriptions;
                if (_subscriptions.TryGetValue(messageType, out messageSubscriptions))
                {
                    toNotify = messageSubscriptions.Values.ToList();
                }
            }

            foreach (ISubscription subscription in toNotify ?? Enumerable.Empty<ISubscription>())
            {
                subscription.Invoke(message);
            }
        }

        public ISubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : IMessage
        {
            if (deliveryAction == null)
                throw new ArgumentNullException(nameof(deliveryAction));

            //Create the subscription
            Type messageType = typeof(TMessage);
            ISubscription subscription = CreateSubscription(deliveryAction);

            //Add the subscription to the subscribers
            lock (_theLock)
            {
                IDictionary<Guid, ISubscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(messageType, out messageSubscriptions))
                {
                    messageSubscriptions = new Dictionary<Guid, ISubscription>();
                    _subscriptions[messageType] = messageSubscriptions;
                }
                messageSubscriptions[subscription.Id] = subscription;
            }

            return CreateSubscriptionToken(subscription.Id, messageType, () => Unsubscribe(subscription.Id, messageType));
        }

        public void Unsubscribe(ISubscriptionToken subscriptionToken)
        {
            Unsubscribe(subscriptionToken.Id, subscriptionToken.MessageType);
        }

        /* ----------------------------------------------------------------------------  */
        /*                                PRIVATE METHODS                                */
        /* ----------------------------------------------------------------------------  */
        private void Unsubscribe(Guid subscriptionTokenId, Type messageType)
        {
            lock (_theLock)
            {
                IDictionary<Guid, ISubscription> messageSubscriptions;

                if (_subscriptions.TryGetValue(messageType, out messageSubscriptions))
                {
                    if (messageSubscriptions.ContainsKey(subscriptionTokenId))
                    {
                        messageSubscriptions.Remove(subscriptionTokenId);
                    }
                }
            }
        }

        private ISubscription CreateSubscription<TMessage>(Action<TMessage> deliveryAction) where TMessage : IMessage
        {
            return new Subscription<TMessage>(deliveryAction);
        }

        public ISubscriptionToken CreateSubscriptionToken(Guid id, Type messageType, Action unsubscribeAction)
        {
            return new SubscriptionToken(id, messageType, unsubscribeAction);
        }

        /* ----------------------------------------------------------------------------  */
        /*                               INTERNAL CLASSES                                */
        /* ----------------------------------------------------------------------------  */
        private class Subscription<TMessage> : ISubscription where TMessage : IMessage
        {
            public Guid Id { get; }

            private readonly Action<TMessage> _deliveryAction;

            public Subscription(Action<TMessage> deliveryAction) : base()
            {
                Id = Guid.NewGuid();
                _deliveryAction = deliveryAction;
            }

            public void Invoke(object message)
            {
                _deliveryAction.Invoke((TMessage)message);
            }
        }

        private class SubscriptionToken : ISubscriptionToken
        {
            public Guid Id { get; }

            public Type MessageType { get; }

            private readonly Action _unsubscribeAction;
            public SubscriptionToken(Guid id, Type messageType, Action unsubscribeAction)
            {
                Id = id;
                MessageType = messageType;

                _unsubscribeAction = unsubscribeAction;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    _unsubscribeAction.Invoke();
                }
            }
        }
    }
}
