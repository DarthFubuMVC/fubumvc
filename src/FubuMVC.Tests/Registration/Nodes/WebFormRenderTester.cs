using System;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;
using FubuMVC.StructureMap;
using FubuMVC.WebForms;
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