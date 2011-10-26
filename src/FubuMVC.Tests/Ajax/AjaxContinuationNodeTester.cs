using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Ajax
{
    [TestFixture]
    public class AjaxContinuationNodeTester
    {
        [Test]
        public void creates_an_ajax_continuation_behavior()
        {
            new AjaxContinuationNode().As<IContainerModel>()
                .ToObjectDef(DiagnosticLevel.None)
                .Type.ShouldEqual(typeof (AjaxContinuationWriter));
        }

        [Test]
        public void node_is_of_category_output()
        {
            new AjaxContinuationNode().Category.ShouldEqual(BehaviorCategory.Output);
        }
    }
}