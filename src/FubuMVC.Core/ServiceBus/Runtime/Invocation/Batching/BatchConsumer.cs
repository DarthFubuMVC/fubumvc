using System.Collections.Generic;

namespace FubuMVC.Core.ServiceBus.Runtime.Invocation.Batching
{
    public abstract class BatchConsumer<T> where T : IBatchMessage
    {
        private readonly IMessageExecutor _executor;

        public BatchConsumer(IMessageExecutor executor)
        {
            _executor = executor;
        }

        public void Handle(T batch)
        {
            BatchStart(batch);

            batch.Messages.Each(x => _executor.Execute(x));

            BatchFinish(batch);
        }

        [NotHandler]
        public virtual void BatchStart(T batch){}

        [NotHandler]
        public virtual void BatchFinish(T batch){}
    }



}