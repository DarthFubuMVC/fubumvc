using System.Linq;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Validation
{
    [TestFixture]
    public class AudienceValidationRuleTester
    {
        private SamlResponse response;
        private AudienceValidationRule theRule;

        [SetUp]
        public void SetUp()
        {
            response = new SamlResponse();

            theRule = new AudienceValidationRule("foo:bar", "bar:foo");
        }

        [Test]
        public void no_conditions_so_it_passes()
        {
            theRule.Validate(response);
            response.Errors.Any().ShouldBeFalse();
        }

        [Test]
        public void one_matching_audience_so_no_errors()
        {
            response.AddAudienceRestriction(theRule.Audiences.ElementAt(0));
            response.AddAudienceRestriction("something:random");

            theRule.Validate(response);
            response.Errors.Any().ShouldBeFalse();
        }

        [Test]
        public void has_audiences_that_do_not_match()
        {
            response.AddAudienceRestriction("something:random");

            theRule.Validate(response);
            response.Errors.Single().ShouldBe(new SamlError(SamlValidationKeys.AudiencesDoNotMatch));
        }
    }
}