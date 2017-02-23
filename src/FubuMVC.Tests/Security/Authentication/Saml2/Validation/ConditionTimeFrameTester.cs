using System.Linq;
using FubuCore;
using FubuCore.Dates;
using FubuMVC.Core.Security.Authentication.Saml2;
using FubuMVC.Core.Security.Authentication.Saml2.Validation;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Saml2.Validation
{
    
    public class ConditionTimeFrameTester
    {
        private SystemTime systemTime   ;
        private ConditionTimeFrame theCondition;
        // INVALID IS
        // if (now < notBefore || now >= notOnOrAfter)
        public ConditionTimeFrameTester()
        {
            systemTime = SystemTime.AtLocalTime("0800".ToTime());
            theCondition = new ConditionTimeFrame(systemTime);
        }

        [Fact]
        public void valid()
        {
            var response = new SamlResponse
            {
                Conditions = new ConditionGroup
                {
                    NotBefore = systemTime.UtcNow().AddMinutes(-5),
                    NotOnOrAfter = systemTime.UtcNow().AddMinutes(5)
                }
            };

            theCondition.Validate(response);
            response.Errors.Any().ShouldBeFalse();
        }

        [Fact]
        public void invalid_because_it_is_before_the_not_before()
        {
            var response = new SamlResponse
            {
                Conditions = new ConditionGroup
                {
                    NotBefore = systemTime.UtcNow().AddMinutes(1),
                    NotOnOrAfter = systemTime.UtcNow().AddMinutes(5)
                }
            };

            theCondition.Validate(response);
            response.Errors.Single().ShouldBe(new SamlError(SamlValidationKeys.TimeFrameDoesNotMatch));
        }

        [Fact]
        public void invalid_because_it_is_equal_to_the_after_time()
        {
            var response = new SamlResponse
            {
                Conditions = new ConditionGroup
                {
                    NotBefore = systemTime.UtcNow().AddMinutes(-1),
                    NotOnOrAfter = systemTime.UtcNow()
                }
            };

            new ConditionTimeFrame(systemTime).Validate(response);
            response.Errors.Single().ShouldBe(new SamlError(SamlValidationKeys.TimeFrameDoesNotMatch));
        }

        [Fact]
        public void invalid_because_it_is_after_to_the_after_time()
        {
            var response = new SamlResponse
            {
                Conditions = new ConditionGroup
                {
                    NotBefore = systemTime.UtcNow().AddMinutes(-5),
                    NotOnOrAfter = systemTime.UtcNow().AddMinutes(-1)
                }
            };

            new ConditionTimeFrame(systemTime).Validate(response);
            response.Errors.Single().ShouldBe(new SamlError(SamlValidationKeys.TimeFrameDoesNotMatch));
        }
    }
}