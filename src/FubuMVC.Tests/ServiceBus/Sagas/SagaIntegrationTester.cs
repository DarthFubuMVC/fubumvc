using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Sagas;
using FubuMVC.Core.ServiceBus.Scheduling;
using FubuMVC.Core.ServiceBus.TestSupport;
using FubuMVC.Core.Services.Messaging.Tracking;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Sagas
{
    [TestFixture, Explicit("About to be converted to ST")]
    public class SagaIntegrationTester
    {
        private SagaLogger theLogger;
        private Container theContainer;
        private FubuRuntime theRuntime;

        [SetUp]
        public void SetUp()
        {

            theLogger = new SagaLogger();
            theContainer = new Container(x =>
            {
                x.For<SagaSettings>().Use(InMemoryTransport.ToInMemory<SagaSettings>());
                x.For<SagaLogger>().Use(theLogger);
                x.For<IListener>().Add<MessageWatcher>();
            });

            var registry = new SagaTestRegistry();
            registry.StructureMap(theContainer);
            registry.Mode = "testing";

            theRuntime = registry.ToRuntime();

            MessageHistory.StartListening();
        }

        [TearDown]
        public void TearDown()
        {
            theRuntime.Dispose();
        }


        [Test]
        public void try_to_run_the_saga_from_beginning_to_end()
        {
            var serviceBus = theContainer.GetInstance<IServiceBus>();
            serviceBus.Send(new TestSagaStart { Name = "Jeremy" });

            Wait.Until(() => !MessageHistory.Outstanding().Any(), timeoutInMilliseconds: 60000);

            var messages = theLogger.Traces.Select(x => x.Message);

            theLogger.Traces.Select(x => x.Id).Distinct()
                     .Count().ShouldBe(1); // should be the same correlation id all the way through

            messages
                .ShouldHaveTheSameElementsAs("Started Jeremy", "Updated Jeremy", "Finished with Updated Jeremy!");
        }

        [Test]
        public void try_to_run_the_saga_from_beginning_to_end_with_implementing_class()
        {
            var serviceBus = theContainer.GetInstance<IServiceBus>();
            serviceBus.Send(new ImplementingClass { Name = "Jeremy" });

            Wait.Until(() => !MessageHistory.Outstanding().Any(), timeoutInMilliseconds: 60000);

            var messages = theLogger.Traces.Select(x => x.Message);

            theLogger.Traces.Select(x => x.Id).Distinct()
                     .Count().ShouldBe(1); // should be the same correlation id all the way through

            messages
                .ShouldHaveTheSameElementsAs("Started Jeremy", "Updated Jeremy", "Finished with Updated Jeremy!");
        }
    }

    [TestFixture]
    public class Stateful_saga_registration_tester
    {
        private SagaLogger theLogger;
        private Container theContainer;
        private FubuRuntime theRuntime;

        [SetUp]
        public void SetUp()
        {
            theLogger = new SagaLogger();
            theContainer = new Container(x =>
            {
                x.For<SagaSettings>().Use(InMemoryTransport.ToInMemory<SagaSettings>());
                x.For<SagaLogger>().Use(theLogger);
                x.For<IListener>().Add<MessageWatcher>();
            });

            var registry = new SagaTestRegistry();
            registry.StructureMap(theContainer);
            

            theRuntime = registry.ToRuntime();

            MessageHistory.ClearAll();
        }

        [TearDown]
        public void Teardown()
        {
            theRuntime.Dispose();
        }

        [Test]
        public void got_the_handler_chains_for_the_saga()
        {
            var graph = theContainer.GetInstance<BehaviorGraph>();
            graph.ChainFor(typeof(TestSagaStart)).ShouldNotBeNull();
            graph.ChainFor(typeof(TestSagaUpdate)).ShouldNotBeNull();
            graph.ChainFor(typeof(TestSagaFinish)).ShouldNotBeNull();
        }

        [Test]
        public void there_is_a_saga_node_with_object_def_for_saga_repository()
        {
            var graph = theContainer.GetInstance<BehaviorGraph>();
            graph.ChainFor(typeof(TestSagaStart)).OfType<StatefulSagaNode>().Single().Repository.ShouldNotBeNull();
            graph.ChainFor(typeof(TestSagaUpdate)).OfType<StatefulSagaNode>().Single().Repository.ShouldNotBeNull();
            graph.ChainFor(typeof(TestSagaFinish)).OfType<StatefulSagaNode>().Single().Repository.ShouldNotBeNull();
        }

    }

    public class SagaTestRegistry : FubuTransportRegistry<SagaSettings>
    {
        public SagaTestRegistry()
        {
            AlterSettings<TransportSettings>(x => x.InMemoryTransport = InMemoryTransportMode.Enabled);

            Channel(x => x.Queue)
                .AcceptsMessagesInAssemblyContainingType<SagaTestRegistry>()
                .ReadIncoming(new ThreadScheduler(2));
        }
    }

    public class SagaSettings
    {
        public Uri Queue { get; set; }
    }

    public class SagaLogger
    {
        public readonly IList<SagaTrace> Traces = new List<SagaTrace>();

        public void Trace(Guid guid, string message)
        {
            Traces.Add(new SagaTrace
            {
                Id = guid,
                Message = message
            });
        }

        public class SagaTrace
        {
            public Guid Id { get; set; }
            public string Message { get; set; }
        }
    }

    public class TestSagaState
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

    }

    public class TestSagaHandler : IStatefulSaga<TestSagaState>
    {
        private readonly SagaLogger _logger;
        private bool _isCompleted;


        public TestSagaHandler(SagaLogger logger)
        {
            _logger = logger;
        }

        public TestSagaState State { get; set; }
        public bool IsCompleted()
        {
            return _isCompleted;
        }

        public void Handle(ImplementingClass message)
        {
            
        }

        public TestSagaUpdate Handle(TestSagaStart start)
        {
            State = new TestSagaState { Id = Guid.NewGuid(), Name = start.Name };
            _logger.Trace(State.Id, "Started " + start.Name);

            return new TestSagaUpdate { CorrelationId = State.Id };
        }

        public TestSagaFinish Handle(TestSagaUpdate update)
        {
            _logger.Trace(State.Id, "Updated " + State.Name);

            State.Name = "Updated " + State.Name;

            return new TestSagaFinish { CorrelationId = State.Id };
        }

        public void Handle(TestSagaFinish finish)
        {
            _isCompleted = true;
            _logger.Trace(State.Id, "Finished with {0}!".ToFormat(State.Name));
        }
    }

    public class TestSagaStart
    {
        public string Name { get; set; }
    }

    public class ImplementingClass : TestSagaStart
    {
        
    }

    public class TestSagaUpdate
    {
        public Guid CorrelationId { get; set; }
    }

    public class TestSagaFinish
    {
        public Guid CorrelationId { get; set; }
    }



}
