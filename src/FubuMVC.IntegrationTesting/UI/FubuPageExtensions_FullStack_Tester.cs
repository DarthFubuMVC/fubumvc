using System;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.View;
using FubuMVC.Tests;
using Xunit;
using Shouldly;

namespace FubuMVC.IntegrationTesting.UI
{
    
    public class FubuPageExtension_with_default_conventions_tester 
    {
        protected string theResult = string.Empty;

        protected void execute(Func<IFubuPage<ConventionTarget>, object> func)
        {
            ConventionEndpoint.Source = func;

            var response = TestHost.Scenario(_ =>
            {
                _.Get.Action<ConventionEndpoint>(x => x.get_result());
                _.StatusCodeShouldBeOk();
            });

            theResult = response.Body.ReadAsText();
        }

        [Fact]
        public void authorized_link_to_positive_directly_against_endpoint_service()
        {
            execute(page =>
            {
                PrincipalRoles.SetCurrentRolesForTesting("Role1");

                return page.AuthorizedLinkTo(svc => svc.EndpointFor<ConventionEndpoint>(x => x.get_authorized_data()));
            });

            theResult.ShouldBe("<a href=\"/authorized/data\"></a>");
        }

        [Fact]
        public void authorized_link_to_negative_directly_against_endpoint_service()
        {
            execute(page =>
            {
                PrincipalRoles.SetCurrentRolesForTesting("Role2");

                return page.AuthorizedLinkTo(svc => svc.EndpointFor<ConventionEndpoint>(x => x.get_authorized_data()));
            });

            theResult.ShouldBe(string.Empty);
        }

        [Fact]
        public void link_to_by_input_model_type()
        {
            execute(page => page.LinkTo<SpecificInput>());
            theResult.ShouldBe("<a href=\"/specific/input\"></a>");
        }

        [Fact]
        public void link_to_by_input_model_gets_full_pattern()
        {
            execute(page => page.LinkTo(new InputWithPattern {Name = "Jeremy"}));
            theResult.ShouldBe("<a href=\"/input/Jeremy\"></a>");
        }

        [Fact]
        public void link_to_by_input_model_that_passes_authorization()
        {
            execute(page =>
            {
                PrincipalRoles.SetCurrentRolesForTesting("Role1");

                return page.LinkTo(new SecuredInput {Name = "Max"});
            });

            theResult.ShouldBe("<a href=\"/secured/by/role/Max\"></a>");
        }

        [Fact]
        public void link_to_by_input_model_that_does_not_pass_authorization()
        {
            execute(page =>
            {
                PrincipalRoles.SetCurrentRolesForTesting("Role2");

                return page.LinkTo(new SecuredInput {Name = "Max"});
            });

            theResult.ShouldBeEmpty();
        }

        [Fact]
        public void link_to_by_controller_name_and_expression()
        {
            execute(page => page.LinkTo<ConventionEndpoint>(x => x.get_result()));

            theResult.ShouldBe("<a href=\"/result\"></a>");
        }

        [Fact]
        public void link_to_new()
        {
            execute(page => page.LinkToNew<Foo>());

            theResult.ShouldBe("<a href=\"/creates/foo\"></a>");
        }

        [Fact]
        public void link_variable_simple()
        {
            execute(page => page.LinkVariable("foo", new InputWithPattern {Name = "Shiner"}));

            theResult.ShouldBe("var foo = '/input/Shiner';");
        }

        [Fact]
        public void link_varuable_by_input_type()
        {
            execute(page => page.LinkVariable<SpecificInput>("foo"));

            theResult.ShouldBe("var foo = '/specific/input';");
        }

        [Fact]
        public void element_name_for()
        {
            execute(page => page.ElementNameFor(x => x.NullableNow));

            theResult.ShouldBe("NullableNow");
        }

        [Fact]
        public void TextboxFor()
        {
            execute(page =>
            {
                page.Model.BigName = "some big name";

                return page.TextBoxFor(x => x.BigName);
            });

            theResult.ShouldBe("<input type=\"text\" name=\"BigName\" value=\"some big name\" />");
        }
    }

    public class ConventionTarget
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public bool Passed { get; set; }
        public string BigName { get; set; }

        public DateTime Now { get; set; }
        public DateTime? NullableNow { get; set; }

        [FakeRequired]
        public string PropWithFakeReqired { get; set; }
        public string PropWithNoAttributes { get; set; }

        [FakeMaximumStringLength(25)]
        public string MaximumLengthProp { get; set; }
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

        [UrlForNew(typeof (Foo))]
        public string get_creates_foo()
        {
            return "hello";
        }
    }

    public class Foo
    {
    }

    public class SpecificInput
    {
    }

    public class InputWithPattern
    {
        public string Name { get; set; }
    }

    public class SecuredInput : InputWithPattern
    {
    }
}