using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class CustomConventionIntegratedTester
    {
        private BehaviorGraph graph = BehaviorGraph.BuildFrom(x =>
        {
            x.Actions.IncludeType<JsonOutputAttachmentTesterController>();

            x.Policies.Local.Add<TestCustomConvention>();
        });

        [Fact]
        public void should_apply_custom_conventions()
        {
            var behavior =
                graph.ChainFor<JsonOutputAttachmentTesterController>(x => x.StringifyHtml()).Calls.First().Next;
            behavior.ShouldBeOfType<OutputNode>();
        }
    }

    public class TestCustomConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Each(call => call.AddToEnd(new OutputNode(typeof (object))));
        }
    }
}