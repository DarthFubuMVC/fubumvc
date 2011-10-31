using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Etags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Etags
{
    [TestFixture]
    public class IfNoneMatchNodeTester
    {
        [Test]
        public void can_build_the_object_def()
        {
            var node = new IfNoneMatchNode(typeof (AssetPath));
            var objectDef = node.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None);

            objectDef.Type.ShouldEqual(
                typeof (OneInOneOutActionInvoker<ETagHandler<AssetPath>, ETaggedRequest, FubuContinuation>));
        }
    }
}