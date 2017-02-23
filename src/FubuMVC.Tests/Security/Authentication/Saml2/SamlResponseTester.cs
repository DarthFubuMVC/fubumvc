using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Security.Authentication.Saml2;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2
{
    
    public class SamlResponseTester
    {
        [Fact]
        public void add_a_single_key_attribute()
        {
            var response = new SamlResponse();
            response.AddAttribute("a", "1");

            response.Attributes.Get("a").ShouldBe("1");
        }

        [Fact]
        public void add_multiple_values_for_the_same_key()
        {
            var response = new SamlResponse();

            response.AddAttribute("a", "1");
            response.AddAttribute("a", "2");
            response.AddAttribute("a", "3");

            response.Attributes.Get("a").As<IEnumerable<string>>()
                .ShouldHaveTheSameElementsAs("1", "2", "3");
        }
    }
}