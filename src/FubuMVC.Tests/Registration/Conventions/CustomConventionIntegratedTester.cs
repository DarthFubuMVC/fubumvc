using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Conneg;
using FubuTestingSupport;
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
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<JsonOutputAttachmentTesterController>();

                x.Policies.Local.Add<TestCustomConvention>();
            });
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void should_apply_custom_conventions()
        {
            var behavior =
                graph.BehaviorFor<JsonOutputAttachmentTesterController>(x => x.StringifyHtml()).Calls.First().Next;
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