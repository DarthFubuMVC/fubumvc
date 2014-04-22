using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Configuration;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
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
            BehaviorNode behavior = graph.BehaviorFor<DownloadTestController>(x => x.Download()).Calls.First().Next;
            var outputNode = behavior.ShouldBeOfType<DownloadFileNode>();
            outputNode.BehaviorType.ShouldEqual(typeof(DownloadFileBehavior));
        }
    }

    public class DownloadTestController
    {
        public DownloadFileModel Download()
        {
            return null;
        }
    }
}