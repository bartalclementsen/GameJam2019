using System;

namespace Core.Mediators
{
    public interface ISubscription
    {
        Guid Id { get; }

        void Invoke(object message);
    }
}
