namespace GDFiddle.Framework.Messaging;

public interface IMessageBus
{
    void Subscribe<TMessage>(Action<TMessage> handler);
    void Publish<TMessage>(TMessage message);
}