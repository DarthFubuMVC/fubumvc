using System;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using FubuTransportation.Configuration;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Testing;
using FubuTransportation.Testing.ScenarioSupport;
using LightningQueues;
using NUnit.Framework;
using StructureMap;

namespace FubuTransportation.LightningQueues.Testing
{
    [TestFixture]
    public class Full_end_to_end_error_processing_with_LightningQueues
    {
        private FubuRuntime _runtime;
        private IServiceBus theServiceBus;
        private IQueueManager _queueManager;
        private OneMessage message1;

        [TestFixtureSetUp]
        public void SetUp()
        {
            TestMessageRecorder.Clear();
            FubuTransport.Reset();

            // Need to do something about this.  Little ridiculous
            var settings = new BusSettings
            {
                Downstream = "lq.tcp://localhost:2040/downstream".ToUri()
            };
            var lightningUri = settings.Downstream.ToLightningUri();

            var container = new Container();
            container.Inject(settings);

            

            _runtime = FubuTransport.For<ErrorRegistry>().StructureMap(container)
                .Bootstrap();
            //_runtime.Factory.Get<IPersistentQueues>().ClearAll();

            theServiceBus = _runtime.Factory.Get<IServiceBus>();

            _queueManager = _runtime.Factory.Get<IPersistentQueues>().ManagerFor(lightningUri.Port, true);

            message1 = new OneMessage();

            theServiceBus.Send(message1);
        }

        [Test]
        public void requeues_then_moves_to_error_queue()
        {
            Wait.Until(() => TestMessageRecorder.HasProcessed(message1)).ShouldBeTrue();
            Wait.Until(() => TestMessageRecorder.AllProcessed.Length == 5).ShouldBeTrue();

            var scope = _queueManager.BeginTransactionalScope();
            var message = scope.Receive(LightningQueuesTransport.ErrorQueueName, 5.Seconds());
            message.ShouldNotBeNull();


            var report = ErrorReport.Deserialize(message.Data);
            message.Headers.Get("ExceptionType").ShouldEqual("System.InvalidOperationException");
            report.RawData.ShouldNotBeNull();
        }


        [TestFixtureTearDown]
        public void TearDown()
        {
            FubuTransport.Reset();
            _runtime.Dispose();
        }
    }

    public class ErrorRegistry : FubuTransportRegistry<BusSettings>
    {
        public ErrorRegistry()
        {
            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<ThrowingHandler<OneMessage>>();
            Channel(x => x.Downstream)
                .ReadIncoming(ByTasks(x => x.DownstreamCount))
                .AcceptsMessagesInAssemblyContainingType<OneMessage>();
            Global.Policy<ErrorPolicy>();
        }
    }

    public class ErrorPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            handlerChain.MaximumAttempts = 5;
            handlerChain.OnException<InvalidOperationException>()
                .Requeue();
        }
    }

    public class ThrowingHandler<T> where T : Message
    {
        public void Handle(T message)
        {
            TestMessageRecorder.Processed(GetType().Name + TestMessageRecorder.AllProcessed.Count(), message);
            throw new InvalidOperationException("blah");
        }
    }
}