using System;

namespace Core.Mediators
{
    public interface ISubscriptionToken : IDisposable
    {
        Guid Id { get; }

        Type MessageType { get; }
    }
}
