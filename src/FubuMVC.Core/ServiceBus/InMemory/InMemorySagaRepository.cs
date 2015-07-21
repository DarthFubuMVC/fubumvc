using System;
using FubuMVC.Core.ServiceBus.Sagas;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    public class InMemorySagaRepository<TState, TMessage> : ISagaRepository<TState, TMessage> where TState : class
    {
        private readonly Func<TMessage, Guid> _messageGetter;
        private readonly Func<TState, Guid> _stateGetter;
        private readonly ISagaStateCacheFactory _cacheFactory;

        public static InMemorySagaRepository<TState, TMessage> Create()
        {
            var types = new SagaTypes
            {
                StateType = typeof (TState),
                MessageType = typeof (TMessage)
            };

            return new InMemorySagaRepository<TState, TMessage>((Func<TMessage, Guid>) types.ToCorrelationIdFunc(), (Func<TState, Guid>) types.ToSagaIdFunc(), new SagaStateCacheFactory());
        } 

        public InMemorySagaRepository(Func<TMessage, Guid> messageGetter, Func<TState, Guid> stateGetter, ISagaStateCacheFactory cacheFactory)
        {
            _messageGetter = messageGetter;
            _stateGetter = stateGetter;
            _cacheFactory = cacheFactory;
        }

        private ISagaStateCache<TState> cache
        {
            get { return _cacheFactory.FindCache<TState>(); }
        } 

        public void Save(TState state, TMessage message)
        {
            cache.Store(_stateGetter(state), state);
        }

        public TState Find(TMessage message)
        {
            var correlationId = _messageGetter(message);
            return cache.Find(correlationId);
        }

        public void Delete(TState state, TMessage message)
        {
            var id = _stateGetter(state);
            cache.Delete(id);
        }
    }
}