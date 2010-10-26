using System;
using System.Reflection;

namespace FubuValidation.Rules
{
    public class GreaterThanZeroValidationRule : IValidationRule
    {
        private readonly PropertyInfo _property;

        public GreaterThanZeroValidationRule(PropertyInfo property)
        {
            _property = property;
        }

        public string Message { get; set; }

        public void Validate(object target, Notification notification)
        {
            var rawValue = _property.GetValue(target, null);
            if(rawValue == null)
            {
                return;
            }

            var value = Convert.ToDecimal(rawValue);
            if (value <= 0)
            {
                notification.RegisterMessage(_property, Message);
            }
        }
    }
}