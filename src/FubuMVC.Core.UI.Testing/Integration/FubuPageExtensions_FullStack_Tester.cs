using System;
using System.Net;
using FubuCore;
using FubuCore.Conversion;
using FubuCore.Reflection;
using FubuLocalization;
using FubuMVC.Core.Security;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.View;
using FubuMVC.TestingHarness;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Core.UI.Testing.Integration
{
    [TestFixture]
    public class FubuPageExtension_with_default_conventions_tester : FubuPageExtensionContext
    {
        protected override void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<ConventionEndpoint>();
        }
        [Test]
        public void HeaderText()
        {
            execute(page => page.HeaderText(x => x.Age));
            theResult.ShouldEqual(LocalizationManager.GetHeader<ConventionTarget>(x => x.Age));
        }

        [Test]
        public void authorized_link_to_positive_directly_against_endpoint_service()
        {
            execute(page => {
                PrincipalRoles.SetCurrentRolesForTesting("Role1");

                return  page.AuthorizedLinkTo(svc => svc.EndpointFor<ConventionEndpoint>(x => x.get_authorized_data()));
            });

            theResult.ShouldEqual("<a href=\"{0}/authorized/data\"></a>".ToFormat(BaseAddress));
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
            theResult.ShouldEqual("<a href=\"{0}/specific/input\"></a>".ToFormat(BaseAddress));
        }

        [Test]
        public void link_to_by_input_model_gets_full_pattern()
        {
            execute(page => page.LinkTo(new InputWithPattern{Name = "Jeremy"}));
            theResult.ShouldEqual("<a href=\"{0}/input/Jeremy\"></a>".ToFormat(BaseAddress));
        }

        [Test]
        public void link_to_by_input_model_that_passes_authorization()
        {
            execute(page => {
                PrincipalRoles.SetCurrentRolesForTesting("Role1");

                return page.LinkTo(new SecuredInput {Name = "Max"});
            });

            theResult.ShouldEqual("<a href=\"{0}/secured/by/role/Max\"></a>".ToFormat(BaseAddress));
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

            theResult.ShouldEqual("<a href=\"{0}/result\"></a>".ToFormat(BaseAddress));
        }

        [Test]
        public void link_to_new()
        {
            execute(page => page.LinkToNew<Foo>());

            theResult.ShouldEqual("<a href=\"{0}/creates/foo\"></a>".ToFormat(BaseAddress));
        }

        [Test]
        public void link_variable_simple()
        {
            execute(page => page.LinkVariable("foo", new InputWithPattern{Name = "Shiner"}));

            theResult.ShouldEqual("var foo = '{0}/input/Shiner';".ToFormat(BaseAddress));
        }

        [Test]
        public void link_varuable_by_input_type()
        {
            execute(page => page.LinkVariable<SpecificInput>("foo"));

            theResult.ShouldEqual("var foo = '{0}/specific/input';".ToFormat(BaseAddress));
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
    public class FubuPageExtensionContext : FubuRegistryHarness
    {
        protected string theResult;

        protected override void beforeRunning()
        {
            theResult = string.Empty;
        }

        protected void execute(Func<IFubuPage<ConventionTarget>, object> func)
        {
            ConventionEndpoint.Source = func;

            var response = endpoints.Get<ConventionEndpoint>(x => x.get_result());
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