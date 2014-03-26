using System;
using System.Net;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class FubuPageExtension_with_default_conventions_tester : FubuPageExtensionContext
    {
        [Test]
        public void authorized_link_to_positive_directly_against_endpoint_service()
        {
            execute(page => {
                PrincipalRoles.SetCurrentRolesForTesting("Role1");

                return  page.AuthorizedLinkTo(svc => svc.EndpointFor<ConventionEndpoint>(x => x.get_authorized_data()));
            });

            theResult.ShouldEqual("<a href=\"/authorized/data\"></a>");
        }

        [Test]
        public void authorized_link_to_negative_directly_against_endpoint_service()
        {
            execute(page =>
            {
                PrincipalRoles.SetCurrentRolesForTesting("Role2");

                return page.AuthorizedLinkTo(svc => svc.EndpointFor<ConventionEndpoint>(x => x.get_authorized_data()));
            });

            theResult.ShouldEqual(string.Empty);
        }

        [Test]
        public void link_to_by_input_model_type()
        {
            execute(page => page.LinkTo<SpecificInput>());
            theResult.ShouldEqual("<a href=\"/specific/input\"></a>");
        }

        [Test]
        public void link_to_by_input_model_gets_full_pattern()
        {
            execute(page => page.LinkTo(new InputWithPattern{Name = "Jeremy"}));
            theResult.ShouldEqual("<a href=\"/input/Jeremy\"></a>");
        }

        [Test]
        public void link_to_by_input_model_that_passes_authorization()
        {
            execute(page => {
                PrincipalRoles.SetCurrentRolesForTesting("Role1");

                return page.LinkTo(new SecuredInput {Name = "Max"});
            });

            theResult.ShouldEqual("<a href=\"/secured/by/role/Max\"></a>");
        }

        [Test]
        public void link_to_by_input_model_that_does_not_pass_authorization()
        {
            execute(page =>
            {
                PrincipalRoles.SetCurrentRolesForTesting("Role2");

                return page.LinkTo(new SecuredInput { Name = "Max" });
            });

            theResult.ShouldBeEmpty();
        }

        [Test]
        public void link_to_by_controller_name_and_expression()
        {
            execute(page => page.LinkTo<ConventionEndpoint>(x => x.get_result()));

            theResult.ShouldEqual("<a href=\"/result\"></a>");
        }

        [Test]
        public void link_to_new()
        {
            execute(page => page.LinkToNew<Foo>());

            theResult.ShouldEqual("<a href=\"/creates/foo\"></a>");
        }

        [Test]
        public void link_variable_simple()
        {
            execute(page => page.LinkVariable("foo", new InputWithPattern{Name = "Shiner"}));

            theResult.ShouldEqual("var foo = '/input/Shiner';");
        }

        [Test]
        public void link_varuable_by_input_type()
        {
            execute(page => page.LinkVariable<SpecificInput>("foo"));

            theResult.ShouldEqual("var foo = '/specific/input';");
        }

        [Test]
        public void element_name_for()
        {
            execute(page => page.ElementNameFor(x => x.NullableNow));

            theResult.ShouldEqual("NullableNow");
        }

        [Test]
        public void TextboxFor()
        {
            execute(page => {
                page.Model.BigName = "some big name";

                return page.TextBoxFor(x => x.BigName);
            });

            theResult.ShouldEqual("<input type=\"text\" name=\"BigName\" value=\"some big name\" />");
        }

    }


    [TestFixture]
    public class FubuPageExtensionContext
    {
        [TestFixtureSetUp]
        public void StartServer()
        {
            var port = PortFinder.FindPort(5500);

            _server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded(port:port);
        }

        [TestFixtureTearDown]
        public void StopServer()
        {
            _server.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            theResult = string.Empty;
        }


        protected string theResult;
        private EmbeddedFubuMvcServer _server;

        public string BaseAddress
        {
            get { return _server.BaseAddress; }
        }


        protected void execute(Func<IFubuPage<ConventionTarget>, object> func)
        {
            ConventionEndpoint.Source = func;

            var response = _server.Endpoints.Get<ConventionEndpoint>(x => x.get_result());
            response.StatusCodeShouldBe(HttpStatusCode.OK);

            theResult = response.ReadAsText();
        }
    }

    public class ConventionEndpoint
    {
        private readonly FubuHtmlDocument<ConventionTarget> _document;
        public static Func<IFubuPage<ConventionTarget>, object> Source = page => "nothing";

        public ConventionEndpoint(FubuHtmlDocument<ConventionTarget> document)
        {
            _document = document;
        }

        public string get_result()
        {
            return Source(_document).ToString();
        }

        [AllowRole("Role1")]
        public string get_authorized_data()
        {
            return "something";
        }

        public string get_specific_input(SpecificInput input)
        {
            return "Hello!";
        }

        public string get_input_Name(InputWithPattern pattern)
        {
            return "Hello";
        }

        public AjaxContinuation post_input_Name(InputWithPattern patter)
        {
            return AjaxContinuation.Successful();
        }

        [AllowRole("Role1")]
        public string get_secured_by_role_Name(SecuredInput input)
        {
            return "Hello";
        }

        [UrlForNew(typeof(Foo))]
        public string get_creates_foo()
        {
            return "hello";
        }
    }

    public class Foo{}

    public class SpecificInput{}

    public class InputWithPattern
    {
        public string Name { get; set; }
    }

    public class SecuredInput : InputWithPattern{}

}