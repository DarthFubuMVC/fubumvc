using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuRegistryTester
    {
        [Test]
        public void what_is_the_order()
        {
            new FubuRegistry();
        }

        [Test]
        public void policies_are_only_registered_once()
        {
            FakePolicy.Count = 0;

            new FubuRegistry(x =>
            {
                x.Policies.Add<FakePolicy>();
                x.Policies.Add<FakePolicy>();
                x.Policies.Add<FakePolicy>();
                x.Policies.Add<FakePolicy>();
                x.Policies.Add<FakePolicy>();
                x.Policies.Add<FakePolicy>();
            }).BuildGraph();

            FakePolicy.Count.ShouldEqual(1);
        }

        [Test]
        public void registries_may_only_be_registered_once()
        {
            FakeIncludeRegistry.Count = 0;

            new FubuRegistry(x =>
            {
                x.Import<FakeIncludeRegistry>(string.Empty);
                x.Import<FakeIncludeRegistry>(string.Empty);
                x.Import<FakeIncludeRegistry>(string.Empty);
                x.Import<FakeIncludeRegistry>(string.Empty);
            }).BuildGraph();

            FakeIncludeRegistry.Count.ShouldEqual(1);
        }
    }

    public class FakeIncludeRegistry : FubuRegistry
    {
        public static int Count;

        public FakeIncludeRegistry()
        {
            Configure(x => Count++);
        }
    }

    public class FakePolicy : IConfigurationAction
    {
        public static int Count;

        public void Configure(BehaviorGraph graph)
        {
            Count++;
        }
    }
}