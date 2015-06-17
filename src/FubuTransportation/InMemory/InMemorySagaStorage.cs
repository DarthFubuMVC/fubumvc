using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTransportation.Sagas;

namespace FubuTransportation.InMemory
{
    public class InMemorySagaStorage : ISagaStorage
    {
        public ObjectDef RepositoryFor(SagaTypes sagaTypes)
        {
            if (sagaTypes.StateType.GetProperty(SagaTypes.Id) == null)
            {
                return null;
            }

            var objectDef = new ObjectDef(typeof (InMemorySagaRepository<,>), sagaTypes.StateType, sagaTypes.MessageType);

            objectDef.DependencyByValue(typeof(Func<,>).MakeGenericType(sagaTypes.StateType, typeof(Guid)), sagaTypes.ToSagaIdFunc());
            objectDef.DependencyByValue(typeof(Func<,>).MakeGenericType(sagaTypes.MessageType, typeof(Guid)), sagaTypes.ToCorrelationIdFunc());

            return objectDef;
        }
    }
}