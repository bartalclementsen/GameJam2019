namespace Core.Mediators
{
    public abstract class Message : IMessage
    {
        public object Sender { get; }

        public Message(object sender)
        {
            Sender = sender;
        }
    }
}
