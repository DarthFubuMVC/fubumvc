using System;
using System.Reflection;

namespace FubuValidation.Rules
{
    public class GreaterOrEqualToZeroValidationRule : IValidationRule
    {
        private readonly PropertyInfo _property;

        public GreaterOrEqualToZeroValidationRule(PropertyInfo property)
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

            double value = Convert.ToDouble(rawValue);
            if (value < 0)
            {
                notification.RegisterMessage(_property, Message);
            }
        }
    }
}