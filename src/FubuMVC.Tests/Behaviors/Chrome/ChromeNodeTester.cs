using FubuCore;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Behaviors.Chrome
{
    [TestFixture]
    public class ChromeNodeTester
    {
        [Test]
        public void chrome_node_creates_behavior()
        {
            var node = new ChromeNode(typeof (ChromeContent));
            var def = node.As<IContainerModel>().ToObjectDef();

            def.Type.ShouldEqual(typeof (ChromeBehavior<ChromeContent>));
        }

        [Test]
        public void chrome_node_creates_behavior_2()
        {
            var node = new ChromeNode(typeof(FakeChrome));
            var def = node.As<IContainerModel>().ToObjectDef();

            def.Type.ShouldEqual(typeof(ChromeBehavior<FakeChrome>));
        }

    }

    public class FakeChrome : ChromeContent{}
}