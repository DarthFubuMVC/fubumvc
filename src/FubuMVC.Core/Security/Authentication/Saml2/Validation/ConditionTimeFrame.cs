using FubuCore.Dates;

namespace FubuMVC.Core.Security.Authentication.Saml2.Validation
{
    public class ConditionTimeFrame : ISamlValidationRule
    {
        private readonly ISystemTime _systemTime;

        public ConditionTimeFrame(ISystemTime systemTime)
        {
            _systemTime = systemTime;
        }

        public void Validate(SamlResponse response)
        {
            var now = _systemTime.UtcNow();
            if (now < response.Conditions.NotBefore || now >= response.Conditions.NotOnOrAfter)
            {
                response.LogError(SamlValidationKeys.TimeFrameDoesNotMatch);
            }
        }
    }
}