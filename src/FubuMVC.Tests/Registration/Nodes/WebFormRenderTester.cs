using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Registration.Nodes
{
    [TestFixture]
    public class WebFormRenderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            path = new ViewPath
            {
                ViewName = "something"
            };

            var render = new WebFormView(path.ViewName);
            var container = new Container(x =>
            {
                x.For<IActionBehavior>().Use(new ObjectDefInstance(render.ToObjectDef()));
                x.For<IWebFormsControlBuilder>().Use<WebFormsControlBuilder>();
                x.For<IWebFormRenderer>().Use<WebFormRenderer>();
                x.For<IOutputWriter>().Use<HttpResponseOutputWriter>();
                x.For<IFubuRequest>().Use<InMemoryFubuRequest>();
                x.For<IViewActivator>().Use<NulloViewActivator>();
            });

            behavior = container.GetInstance<IActionBehavior>();
        }

        #endregion

        private ViewPath path;
        private IActionBehavior behavior;

        [Test]
        public void the_behavior_should_be_a_render_fubu_web_form_view()
        {
            behavior.ShouldBeOfType<RenderFubuWebFormView>().View.ShouldEqual(path);
        }
    }
}