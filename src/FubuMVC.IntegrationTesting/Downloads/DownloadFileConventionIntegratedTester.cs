using System.Linq;
using FubuMVC.Core.Downloads;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Downloads
{
    [TestFixture]
    public class DownloadFileConventionIntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<DownloadTestController>();

                x.Policies.Local.Add<DownloadFileConvention>();
            });
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void should_apply_download_behavior_convention()
        {
            BehaviorNode behavior = graph.ChainFor<DownloadTestController>(x => x.Download()).Calls.First().Next;
            var outputNode = behavior.ShouldBeOfType<DownloadFileNode>();
            outputNode.BehaviorType.ShouldBe(typeof(DownloadFileBehavior));
        }
    }
}