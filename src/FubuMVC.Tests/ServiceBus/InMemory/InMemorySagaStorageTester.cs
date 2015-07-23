using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Sagas;
using NUnit.Framework;
using FubuTestingSupport;
using StructureMap.Pipeline;
using Message1 = FubuTransportation.Testing.Message1;

namespace FubuTransportation.Testing.InMemory
{


    [TestFixture]
    public class InMemorySagaStorageTester
    {
        [Test]
        public void sad_path()
        {
            new InMemorySagaStorage().RepositoryFor(new SagaTypes
            {
                MessageType = GetType(),
                StateType = GetType()
            }).ShouldBeNull();
        }
    }

    public class MySagaState
    {
        public Guid Id { get; set; }
    }

    public class SagaMessageOne
    {
        public Guid CorrelationId { get; set; }
    }

    public class SagaMessageTwo
    {
        public Guid CorrelationId { get; set; }
    }

    public class SagaMessageThree
    {
        public Guid CorrelationId { get; set; }
    }

    public class SimpleSagaHandler : IStatefulSaga<Sagas.MySagaState>
    {
        public bool IsCompleted()
        {
            throw new NotImplementedException();
        }

        public Sagas.MySagaState State { get; set; }

        public void Start(Sagas.SagaMessageOne one)
        {

        }

        public void Second(Sagas.SagaMessageTwo two)
        {

        }

        public void Last(Sagas.SagaMessageThree three)
        {

        }
    }
}