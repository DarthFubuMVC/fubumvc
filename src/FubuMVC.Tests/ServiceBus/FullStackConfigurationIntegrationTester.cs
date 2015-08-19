using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class FullStackConfigurationIntegrationTester
    {
        [Test]
        public void has_all_the_chains_we_expect()
        {
            using (var runtime = FubuRuntime.For<MyFirstTransport>())
            {

                var graph = runtime.Get<BehaviorGraph>();

                graph.Chains.Count(x => typeof (Foo1) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
                graph.Chains.Count(x => typeof(Foo2) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
                graph.Chains.Count(x => typeof(Foo3) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
                graph.Chains.Count(x => typeof(Foo4) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
            }
        }

        [Test]
        public void has_all_the_chains_we_expect_through_FubuApplication()
        {
            using (var runtime = FubuRuntime.For<MyFirstTransport>())
            {

                var graph = runtime.Get<BehaviorGraph>();

                graph.Chains.Count(x => typeof(Foo1) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
                graph.Chains.Count(x => typeof(Foo2) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
                graph.Chains.Count(x => typeof(Foo3) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
                graph.Chains.Count(x => typeof(Foo4) == x.InputType()).ShouldBeGreaterThanOrEqualTo(1);
            }
        }
    }


    public class MyFirstTransport : FubuRegistry
    {
        public MyFirstTransport()
        {
            ServiceBus.EnableInMemoryTransport();
            ServiceBus.Enable(true);
        }
    }


    public class MyConsumer
    {
        public void Foo1(Foo1 input) { }
        public void Foo2(Foo2 input) { }
        public void Foo3(Foo3 input) { }
    }

    public class MyOtherConsumer
    {
        public void Foo2(Foo2 input) { }
        public void Foo3(Foo3 input) { }
        public void Foo4(Foo4 input) { }
    }

    public class Foo1 { }
    public class Foo2 { }
    public class Foo3 { }
    public class Foo4 { }

}