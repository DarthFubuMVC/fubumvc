using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class HandlerGraphTester
    {
        private HandlerGraph theGraph;
        private HandlerCall concreteCall;
        private HandlerCall concreteCall2;
        private HandlerCall concreteCall3;
        private HandlerCall concreteCall4;

        [SetUp]
        public void SetUp()
        {
            theGraph = new HandlerGraph();

            concreteCall = HandlerCall.For<ConcreteHandler>(x => x.M1(null));

            concreteCall.Clone().ShouldBe(concreteCall);


            concreteCall2 = HandlerCall.For<ConcreteHandler>(x => x.M2(null));
            concreteCall3 = HandlerCall.For<ConcreteHandler>(x => x.M3(null));
            concreteCall4 = HandlerCall.For<ConcreteHandler>(x => x.M4(null));
        }

        [Test]
        public void add_a_handler_for_a_concrete_class_creates_a_new_chain()
        {
            theGraph.Add(concreteCall);

            var call = theGraph.ChainFor(typeof (Input)).OfType<HandlerCall>().Single();
            call.ShouldBe(concreteCall);
            call.ShouldNotBeTheSameAs(concreteCall);
        }

        [Test]
        public void add_a_second_handler_for_a_concrete_class_appends_to_the_chain()
        {
            theGraph.Add(concreteCall);
            theGraph.Add(concreteCall2);
            theGraph.Add(concreteCall3);

            theGraph.ShouldHaveCount(1);

            var calls = theGraph.ChainFor(typeof (Input)).OfType<HandlerCall>().ToArray();
            calls.ShouldHaveCount(3);
            var firstCall = calls.ElementAt(0).ShouldBeOfType<HandlerCall>();

            firstCall.Equals(concreteCall).ShouldBeTrue();

            calls.ElementAt(0).ShouldNotBeTheSameAs(concreteCall);

            calls.ElementAt(1).Equals(concreteCall2).ShouldBeTrue();
            calls.ElementAt(1).ShouldNotBeTheSameAs(concreteCall2);

        }

        [Test]
        public void add_a_different_input_type_adds_a_second_chain()
        {
            theGraph.Add(concreteCall);
            theGraph.Add(concreteCall2);
            theGraph.Add(concreteCall4);
            theGraph.Add(concreteCall3);

            theGraph.ShouldHaveCount(2);

            theGraph.Select(x => x.InputType())
                .ShouldHaveTheSameElementsAs(typeof(Input), typeof(DifferentInput));
        }

        [Test]
        public void interfaces_are_applied_correctly()
        {
            var general = HandlerCall.For<ConcreteHandler>(x => x.General(null));
            var specific1 = HandlerCall.For<ConcreteHandler>(x => x.Specific1(null));
            var specific2 = HandlerCall.For<ConcreteHandler>(x => x.Specific2(null));
        
            theGraph.Add(general);
            theGraph.Add(specific1);
            theGraph.Add(specific2);

            theGraph.ShouldHaveCount(2);
            theGraph.Compile();

            theGraph.ShouldHaveCount(2);

            theGraph.ChainFor(typeof (Concrete1)).Last().ShouldBeOfType<HandlerCall>()
                    .InputType().ShouldBe(typeof (IMessage));

            theGraph.ChainFor(typeof(Concrete2)).Last().ShouldBeOfType<HandlerCall>()
                    .InputType().ShouldBe(typeof(IMessage));
        }

        [Test]
        public void base_class_handlers_are_applied_correctly()
        {
            var baseHandler = HandlerCall.For<ConcreteHandler>(x => x.Base(null));
            var derivedHandler = HandlerCall.For<ConcreteHandler>(x => x.Derived(null));

            theGraph.Add(baseHandler);
            theGraph.Add(derivedHandler);

            theGraph.Compile();

            theGraph.ShouldHaveCount(1);

            theGraph.ChainFor(typeof(DerivedMessage)).Last().ShouldBeOfType<HandlerCall>()
                .Equals(baseHandler).ShouldBeTrue();
        }

        [Test]
        public void merging_adds_a_chain_for_all_new_message_type()
        {
            theGraph.Add(concreteCall);

            var other = new HandlerGraph();
            other.Add(concreteCall4);

            theGraph.Import(other);

            theGraph.Select(x => x.InputType())
                .ShouldHaveTheSameElementsAs(typeof(Input), typeof(DifferentInput));
        }

        [Test]
        public void merging_matching_chains_merges_the_handlers_for_the_same_message()
        {
            theGraph.Add(concreteCall);

            var other = new HandlerGraph();
            other.Add(concreteCall2);
            other.Add(concreteCall3);

            theGraph.Import(other);

            theGraph.ShouldHaveCount(1);

            var chain = theGraph.ChainFor(typeof (Input));
            chain.ElementAt(0).Equals(concreteCall).ShouldBeTrue();
            chain.ElementAt(1).Equals(concreteCall2).ShouldBeTrue();
            chain.ElementAt(2).Equals(concreteCall3).ShouldBeTrue();

        }

        [Test]
        public void applies_general_action_from_imported_graph()
        {
            var general = HandlerCall.For<ConcreteHandler>(x => x.General(null));
            var specific1 = HandlerCall.For<ConcreteHandler>(x => x.Specific1(null));
            var specific2 = HandlerCall.For<ConcreteHandler>(x => x.Specific2(null));
        
            theGraph.Add(specific1);

            var other = new HandlerGraph();
            other.Add(general);
            other.Add(specific2);

            theGraph.Import(other);

            theGraph.Compile();

            theGraph.ChainFor(typeof(Concrete1)).Last()
                .Equals(general).ShouldBeTrue();

            theGraph.ChainFor(typeof(Concrete2)).Last()
                .Equals(general).ShouldBeTrue();


        }

        [Test]
        public void compile_applies_modify_chain_attributes()
        {
            var specific1 = HandlerCall.For<ConcreteHandler>(x => x.Specific1(null));
            var specific2 = HandlerCall.For<ConcreteHandler>(x => x.Specific2(null));

            theGraph.Add(specific1);
            theGraph.Add(specific2);

            theGraph.Compile();

            theGraph.ChainFor<Concrete1>().IsWrappedBy(typeof(BlueWrapper)).ShouldBeTrue();
            theGraph.ChainFor<Concrete2>().IsWrappedBy(typeof(GreenWrapper)).ShouldBeTrue();
        }
    }

    public class ConcreteHandler
    {
        public void M1(Input input){}
        public void M2(Input input){}
        public void M3(Input input){}

        public void M4(DifferentInput input)
        {
        }

        public void General(IMessage input){}

        [WrapWith(typeof(BlueWrapper))]
        public void Specific1(Concrete1 input){}

        [WrapWith(typeof(GreenWrapper))]
        public void Specific2(Concrete2 input){}

        public void Base(BaseMesage message){}
        public void Derived(DerivedMessage message){}
    }

    public interface IMessage{}
    public class Concrete1 : IMessage{}
    public class Concrete2 : IMessage{}

    public abstract class BaseMesage
    {
    }

    public class DerivedMessage : BaseMesage{}
}