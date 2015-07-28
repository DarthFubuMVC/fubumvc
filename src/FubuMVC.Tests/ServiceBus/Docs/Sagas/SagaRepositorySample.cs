using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Sagas;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.ServiceBus.Docs.Sagas
{
    // SAMPLE: SagaRepositorySample
    public class SagaRepositorySampleTransportRegistry : FubuRegistry
    {
        public SagaRepositorySampleTransportRegistry()
        {
            //if overriding the repository of a specific chain
            Policies.Local.Add<AlternativeRepositoryPolicy>();

            //or if globally overriding all saga persistence
            ServiceBus.SagaStorage<AlternativeSagaStorage>();
        }
    }

    public class AlternativeSagaStorage : ISagaStorage
    {
        public Instance RepositoryFor(SagaTypes sagaTypes)
        {
            //A good place if needing something like RavenDbSagaRepository
            //The ObjectDef returned here will be a class that implements ISagaRepository
            //InMemorySagaRepository is a good example of one to look at
            return new ConfiguredInstance(typeof(SagaRepositorySample));
        }
    }

    public class AlternativeRepositoryPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.OfType<StatefulSagaNode>().Each(x =>
            {
                x.Repository = new ConfiguredInstance(typeof(SagaRepositorySample));
            });
        }

        public override bool Matches(HandlerChain chain)
        {
            //You should apply condition to filter 'custom' repository to only the chain the needs it
            return true;
        }
    }

    public class SagaRepositorySample : ISagaRepository<DummyState, StartMessage>
    {
        public void Save(DummyState state, StartMessage message)
        {
            //Save changes to storage
        }

        public DummyState Find(StartMessage message)
        {
            //Passes the message for alternative lookup or key strategies.
            //If the message has CorrelationId:Guid a custom saga repository is not necessary
            //If state is found, the saga has already been started
            return null;
        }

        public void Delete(DummyState state, StartMessage message)
        {
            //The saga was completed, now delete or expire the saga instance
        }
    }

    public class StartMessage
    {
        public Guid CorrelationId { get; set; }
    }

    public class DummyState
    {
        public bool IsImportant { get; set; }
    }
    // ENDSAMPLE
}