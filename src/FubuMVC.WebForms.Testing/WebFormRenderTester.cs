using System;
using FubuMVC.Core.Behaviors;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.WebForms.Testing
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

            throw new NotImplementedException("NWO");

            //var container = new Container(x =>
            //{
            //    x.For<IActionBehavior>().Use(new ObjectDefInstance(render.As<IContainerModel>().ToObjectDef(DiagnosticLevel.None)));
            //    x.For<IWebFormsControlBuilder>().Use<WebFormsControlBuilder>();
            //    x.For<IWebFormRenderer>().Use<WebFormRenderer>();
            //    x.For<HttpContextBase>().Use(() => new FakeHttpContext());
            //    x.For<IOutputWriter>().Use<OutputWriter>();
            //    x.For<IFubuRequest>().Use<InMemoryFubuRequest>();
            //    x.For<IPageActivator>().Use<PageActivator>();
            //    x.For<IPageActivationRules>().Use<PageActivationRuleCache>();
            //    x.For<IServiceLocator>().Use<StructureMapServiceLocator>();
            //    x.For<IHttpWriter>().Use(new NulloHttpWriter());
            //    x.For<IFileSystem>().Use<FileSystem>();

            //});

            //behavior = container.GetInstance<IActionBehavior>();
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