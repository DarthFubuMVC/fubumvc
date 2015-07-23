using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class client_message_mechanics_Tester
    {
        [Test]
        public void is_client_message_false_with_no_att_or_base_class_match()
        {
            GetType().IsClientMessage().ShouldBeFalse();
        }

        [Test]
        public void is_client_message_true_when_derived_from_client_message()
        {
            typeof(ClientA).IsClientMessage().ShouldBeTrue();
        }

        [Test]
        public void is_client_message_with_att()
        {
            typeof(ClientC).IsClientMessage().ShouldBeTrue();
        }

        [Test]
        public void get_message_name()
        {
            typeof (ClientA).GetMessageName().ShouldBe("a");
            typeof (ClientB).GetMessageName().ShouldBe("b");
            typeof (ClientC).GetMessageName().ShouldBe("c");
            typeof (NotClient).GetMessageName().ShouldBe("not-client");
        }

        [Test]
        public void chain_is_aggregated_chain()
        {
            BehaviorChain.For<ChainTarget>(x => x.get_B(null)).IsAggregatedChain().ShouldBeTrue();
            BehaviorChain.For<ChainTarget>(x => x.get_C1()).IsAggregatedChain().ShouldBeTrue();
            BehaviorChain.For<ChainTarget>(x => x.get_C2(null)).IsAggregatedChain().ShouldBeTrue();
            BehaviorChain.For<ChainTarget>(x => x.get_not_client()).IsAggregatedChain().ShouldBeFalse();
        }
    }

    public class ChainTarget
    {
        public ClientB get_B(ClientA input)
        {
            return new ClientB();
        }

        public ClientC get_C1()
        {
            return new ClientC();
        }

        public ClientC get_C2(NotClient input)
        {
            return new ClientC();
        }

        public NotClient get_not_client()
        {
            return new NotClient();
        }
    }

    public class ClientA : ClientMessage
    {
        public ClientA() : base("a")
        {
        }
    }

    public class ClientB : ClientMessage
    {
        public ClientB()
            : base("b")
        {
        }
    }

    [ClientMessage("c")]
    public class ClientC
    {
        
    }

    public class NotClient { }
}