using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class RenderTextNodeTester
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void render_text_node_should_add_mime_to_children_on_configure_object()
        {
            var text = new RenderTextNode<RouteParameter>();
            ObjectDef def = text.As<IContainerModel>().ToObjectDef();
            def.Dependencies.ShouldHaveCount(1).ShouldContain(
                dependency =>
                {
                    var valueDependency = dependency as ValueDependency;
                    return dependency.DependencyType == typeof (MimeType) &&
                    valueDependency != null && valueDependency.Value == MimeType.Text;
                });
        }
    }
}