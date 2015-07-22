using System;
using FubuMVC.Core.ServiceBus.Sagas;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    public class InMemorySagaStorage : ISagaStorage
    {
        public Instance RepositoryFor(SagaTypes sagaTypes)
        {
            if (sagaTypes.StateType.GetProperty(SagaTypes.Id) == null)
            {
                return null;
            }

            var instance = new ConfiguredInstance(typeof (InMemorySagaRepository<,>), sagaTypes.StateType, sagaTypes.MessageType);
            instance.Dependencies.Add(typeof(Func<,>).MakeGenericType(sagaTypes.StateType, typeof(Guid)), sagaTypes.ToSagaIdFunc());
            instance.Dependencies.Add(typeof(Func<,>).MakeGenericType(sagaTypes.MessageType, typeof(Guid)), sagaTypes.ToCorrelationIdFunc());


            return instance;
        }
    }
}