using Core.Containers;

namespace Core.Mediators
{
    public static class MessengerExtensions
    {
        public static ContainerBuilder RegisterMessenger(this ContainerBuilder builder)
        {
            builder.Register<IMessenger, Messenger>();
            return builder;
        }
    }
}
