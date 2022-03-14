namespace GDFiddle.Framework.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<object>> _handlerListPerMessageType;

        public MessageBus()
        {
            _handlerListPerMessageType = new Dictionary<Type, List<object>>();
        }

        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (!_handlerListPerMessageType.TryGetValue(typeof(TMessage), out var handlers))
            {
                _handlerListPerMessageType.Add(typeof(TMessage), new List<object>() { handler });
            }
            else
            {
                handlers.Add(handler);
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            if (_handlerListPerMessageType.TryGetValue(typeof(TMessage), out var handlerList))
            {
                handlerList.ForEach(handler => ((Action<TMessage>)handler).Invoke(message));
            }
        }
    }
}
