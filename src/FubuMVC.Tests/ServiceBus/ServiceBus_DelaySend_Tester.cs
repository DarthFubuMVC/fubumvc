using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Tests.TestSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class ServiceBus_DelaySend_Tester : InteractionContext<Core.ServiceBus.ServiceBus>
    {
        protected override void beforeEach()
        {
            LocalSystemTime = DateTime.Today.AddHours(4);
        }

        private Envelope theLastEnvelopeSent
        {
            get
            {
                return MockFor<IEnvelopeSender>().GetArgumentsForCallsMadeOn(x => x.Send(null))
                    .Last()[0].As<Envelope>();
            }
        }

        [Test]
        public void send_by_a_time()
        {
            var expectedTime = DateTime.Today.AddHours(5);
            var theMessage = new Message1();
            ClassUnderTest.DelaySend(theMessage, expectedTime);

            theLastEnvelopeSent.Message.ShouldBeTheSameAs(theMessage);
            theLastEnvelopeSent.ExecutionTime.ShouldBe(expectedTime.ToUniversalTime());
        }

        [Test]
        public void send_by_delay()
        {
            var theMessage = new Message1();
            ClassUnderTest.DelaySend(theMessage, 5.Hours());

            theLastEnvelopeSent.Message.ShouldBeTheSameAs(theMessage);
            theLastEnvelopeSent.ExecutionTime.ShouldBe(UtcSystemTime.AddHours(5));
        }
    }
}