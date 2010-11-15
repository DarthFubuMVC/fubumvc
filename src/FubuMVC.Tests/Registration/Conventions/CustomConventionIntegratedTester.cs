using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class CustomConventionIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Actions.IncludeTypesImplementing<JsonOutputAttachmentTesterController>();

                x.ApplyConvention<TestCustomConvention>();
            })
                .BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void should_apply_custom_conventions()
        {
            BehaviorNode behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.StringifyHtml()).Calls.First().Next;
            behavior.ShouldBeOfType<RenderJsonNode>();
        }
    }

    public class TestCustomConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Each(call => call.AddToEnd(new RenderJsonNode(typeof (object))));
        }
    }
}