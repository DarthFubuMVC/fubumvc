using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.ServiceBus.Runtime;
using Shouldly;
using NUnit.Framework;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus.Runtime
{
    [TestFixture]
    public class EnvelopeModifierTester
    {
        [Test]
        public void abstract_modifier_is_actually_useful()
        {
            FakeEnvelopeModifier.Modified.Clear();

            var modifier = new FakeEnvelopeModifier();

            modifier.Modify(new Envelope{Message = new Message()});
            modifier.Modify(new Envelope{Message = new OneMessage()});
            modifier.Modify(new Envelope{Message = new TwoMessage()});
            modifier.Modify(new Envelope{Message = new GreenFoo()});

            FakeEnvelopeModifier.Modified.Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(Message), typeof(OneMessage), typeof(TwoMessage));
        }
    }

    public class FakeEnvelopeModifier : EnvelopeModifier<Message>
    {
        public static readonly IList<Message> Modified = new List<Message>();

        public override void Modify(Envelope envelope, Message target)
        {
            Modified.Add(target);
        }
    }

    public class GreenFoo { }
}