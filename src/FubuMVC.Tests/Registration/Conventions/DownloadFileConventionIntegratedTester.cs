using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
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
            graph = new FubuRegistry(x =>
            {
                x.Actions.IncludeTypesImplementing<DownloadTestController>();

                x.ApplyConvention<DownloadFileConvention>();
            })
                .BuildGraph();
        }

        #endregion

        private BehaviorGraph graph;

        [Test]
        public void should_apply_download_behavior_convention()
        {
            BehaviorNode behavior = graph.BehaviorFor<DownloadTestController>(x => x.Download()).Calls.First().Next;
            var outputNode = behavior.ShouldBeOfType<OutputNode>();
            outputNode.BehaviorType.ShouldEqual(typeof (DownloadFileBehavior));
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