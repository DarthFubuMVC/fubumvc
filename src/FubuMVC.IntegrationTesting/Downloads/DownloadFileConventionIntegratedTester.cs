using System.Linq;
using FubuMVC.Core.Downloads;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Xunit;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Downloads
{
    
    public class DownloadFileConventionIntegratedTester
    {
        private BehaviorGraph graph = BehaviorGraph.BuildFrom(x =>
        {
            x.Actions.IncludeType<DownloadTestController>();

            x.Policies.Local.Add<DownloadFileConvention>();
        });

        [Fact]
        public void should_apply_download_behavior_convention()
        {
            BehaviorNode behavior = graph.ChainFor<DownloadTestController>(x => x.Download()).Calls.First().Next;
            var outputNode = behavior.ShouldBeOfType<DownloadFileNode>();
            outputNode.BehaviorType.ShouldBe(typeof(DownloadFileBehavior));
        }
    }
}