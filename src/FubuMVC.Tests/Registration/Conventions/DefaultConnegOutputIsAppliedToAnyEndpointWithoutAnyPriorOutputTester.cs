using FubuMVC.Core;
using FubuMVC.Core.Runtime.Formatters;
using NUnit.Framework;
using FubuTestingSupport;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultConnegOutputIsAppliedToAnyEndpointWithoutAnyPriorOutputTester
    {
        [Test]
        public void applies_conneg()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<SomeController>();

            var chain = registry.BuildGraph().BehaviorFor<SomeController>(x => x.Go(null));

            chain.Output.UsesFormatter<JsonFormatter>();
            chain.Output.UsesFormatter<XmlFormatter>();

            chain.Input.AllowHttpFormPosts.ShouldBeTrue();
            chain.Input.UsesFormatter<JsonFormatter>();
            chain.Input.UsesFormatter<XmlFormatter>();
        }

        public class SomeInput{}
        public class SomeResource{}

        public class SomeController
        {
            public SomeResource Go(SomeInput input)
            {
                return new SomeResource();
            }
        }
    }
}