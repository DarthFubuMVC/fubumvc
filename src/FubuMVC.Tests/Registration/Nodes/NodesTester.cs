using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
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
            var text = new RenderTextNode<RouteInput>();
            ObjectDef def = text.ToObjectDef();
            def.Dependencies.ShouldHaveCount(1).ShouldContain(
                dependency =>
                {
                    var valueDependency = dependency as ValueDependency;
                    return dependency.DependencyType == typeof (MimeType) &&
                    valueDependency != null && valueDependency.Value == MimeType.Text;
                });
        }
    }

    [TestFixture]
    public class OutputNodeTester
    {
        private OutputNode _node;
        private Type _behaviorType;

        [SetUp]
        public void SetUp()
        {
            _behaviorType = typeof(BasicBehavior);
            _node = OutputNode.For<BasicBehavior>();
        }

        [Test]
        public void for_instantiate_new_node()
        {
            _node.BehaviorType.ShouldEqual(_behaviorType);
        }

        [Test]
        public void description_returns_behavior_type_full_name()
        {
            _node.Description.ShouldEqual(_behaviorType.Name);
        }

        [Test]
        public void to_string_returns_description()
        {
            _node.ToString().ShouldEqual(_behaviorType.Name);
        }
    }

    [TestFixture]
    public class RenderJsonNodeTester
    {
        private RenderJsonNode node;

        public class FakeJsonModel {}

        [SetUp]
        public void SetUp()
        {
            node = new RenderJsonNode(typeof(FakeJsonModel));
        }

        [Test]
        public void should_tell_if_equal()
        {
            node.Equals(null).ShouldBeFalse();
            node.Equals(node).ShouldBeTrue();
            node.Equals((object)new RenderJsonNode(typeof(FakeJsonModel))).ShouldBeTrue();
            node.Equals((object)null).ShouldBeFalse();
            node.Equals((object)node).ShouldBeTrue();
            node.Equals("").ShouldBeFalse();
        }

        [Test]
        public void should_get_model_type_hash_code()
        {
            node.GetHashCode().ShouldEqual(typeof (FakeJsonModel).GetHashCode());
        }
    }

    [TestFixture]
    public class RenderHtmlTagNodeTester
    {
        private RenderHtmlTagNode node;

        [SetUp]
        public void SetUp()
        {
            node = new RenderHtmlTagNode();
        }

        [Test]
        public void description_should_return_write_html_tag()
        {
            node.Description.ShouldEqual("Write HtmlTag");
        }
    }
}