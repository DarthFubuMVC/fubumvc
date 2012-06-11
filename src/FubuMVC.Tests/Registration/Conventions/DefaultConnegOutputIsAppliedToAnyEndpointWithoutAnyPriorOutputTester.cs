using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class DefaultConnegOutputIsAppliedToAnyEndpointWithoutAnyPriorOutputTester
    {
        public class SomeInput
        {
        }

        public class SomeResource
        {
        }

        public class SomeController
        {
            public SomeResource Go(SomeInput input)
            {
                return new SomeResource();
            }
        }

        [Test]
        public void applies_conneg()
        {
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<SomeController>();

            var chain = BehaviorGraph.BuildFrom(registry).BehaviorFor<SomeController>(x => x.Go(null));

            chain.Output.UsesFormatter<JsonFormatter>();
            chain.Output.UsesFormatter<XmlFormatter>();

            chain.Input.AllowHttpFormPosts.ShouldBeTrue();
            chain.Input.UsesFormatter<JsonFormatter>();
            chain.Input.UsesFormatter<XmlFormatter>();
        }
    }
}