using System;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.ServiceBus.Sagas
{
    public class SagaBehavior<TState, TMessage, THandler> : WrappingBehavior where THandler : IStatefulSaga<TState>
                                                                             where TMessage : class
                                                                             where TState : class
    {
        private readonly IFubuRequest _request;
        private readonly ISagaRepository<TState, TMessage> _repository;
        private readonly THandler _handler;

        public SagaBehavior(IFubuRequest request, ISagaRepository<TState, TMessage> repository, THandler handler)
        {
            _request = request;
            _repository = repository;
            _handler = handler;
        }

        protected override async Task invoke(Func<Task> func)
        {
            var message = _request.Find<TMessage>().FirstOrDefault();
            if (message == null)
            {
                throw new Exception($"Message of type {typeof(TMessage)} is required.");
            }

            _handler.State = _repository.Find(message);

            await func().ConfigureAwait(false);

            if (_handler.State == null) return;

            if (_handler.IsCompleted())
            {
                _repository.Delete(_handler.State, message);
            }
            else
            {
                _repository.Save(_handler.State, message);
            }
        }
    }
}