namespace FubuMVC.Core.ServiceBus.Sagas
{
    public interface ISagaRepository<TState, TMessage>
    {
        // Want the message in the signature so the Id doesn't have
        // to be duplicated
        void Save(TState state, TMessage message);
        TState Find(TMessage message);
        void Delete(TState state, TMessage message);
    }
}