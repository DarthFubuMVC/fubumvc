using System;
using System.Net;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class FormRequestConventionsIntegrationTester
    {
        private EmbeddedFubuMvcServer _server;

        [TestFixtureSetUp]
        public void StartServer()
        {
            var port = PortFinder.FindPort(5500);

            _server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded(port: port);
        }

        [TestFixtureTearDown]
        public void StopServer()
        {
            _server.Dispose();
        }

        private void runPage()
        {
            _server.Endpoints.Get<FormRequestEndpoint>(x => x.get_form_conventions())
                     .StatusCodeShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void by_model_input()
        {
            var input = new FormInput {Name = "Scooby"};
            FormRequestEndpoint.Source = page => page.FormFor(input);

            runPage();

            FormRequestEndpoint.LastTag.ToString().ShouldEqual("<form method=\"post\" action=\"/form/Scooby\">");
        }

        [Test]
        public void by_controller_method()
        {
            FormRequestEndpoint.Source = page => page.FormFor<FormRequestEndpoint>(x => x.post_update_target(null));

            runPage();

            FormRequestEndpoint.LastTag.ToString().ShouldEqual("<form method=\"post\" action=\"/update/target\">");
        }

        [Test]
        public void by_model_type()
        {
            FormRequestEndpoint.Source = page => page.FormFor<PostedData>();

            runPage();

            FormRequestEndpoint.LastTag.ToString().ShouldEqual("<form method=\"post\" action=\"/update/target\">");
        }
    }


    public class FormRequestEndpoint
    {
        private readonly FubuHtmlDocument<FormTagModel> _document;
        public static Func<IFubuPage, HtmlTag> Source;

        public FormRequestEndpoint(FubuHtmlDocument<FormTagModel> document)
        {
            _document = document;
        }

        public static HtmlTag Build(IFubuPage page)
        {
            LastTag = Source(page);
            return LastTag;
        }

        public static HtmlTag LastTag { get; set; }

        public HtmlDocument get_form_conventions()
        {
            LastTag = Source(_document);
            _document.Add(LastTag);

            return _document;
        }

        public AjaxContinuation post_update_target(PostedData data)
        {
            return AjaxContinuation.Successful();
        }

        public AjaxContinuation get_different(FormInput input)
        {
            return AjaxContinuation.Successful();
        }

        public AjaxContinuation post_form_Name(FormInput input)
        {
            return AjaxContinuation.Successful();
        }
    }

    public class PostedData{}

    public class FormTagModel{}

    public class FormTagModelDocument : FubuHtmlDocument<FormTagModel>
    {
        public FormTagModelDocument(IServiceLocator services, IFubuRequest request) : base(services, request)
        {
            HtmlTag formTag = FormRequestEndpoint.Build(this);
            

            Add(formTag);
        }
    }

    public class FormInput
    {
        public string Name { get; set; }
    }
}