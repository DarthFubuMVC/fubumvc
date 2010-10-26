using System.Reflection;

namespace FubuValidation.Rules
{
    public class MaximumStringLengthValidationRule : IValidationRule
    {
        private readonly PropertyInfo _property;
        private readonly int _length;

        public MaximumStringLengthValidationRule(PropertyInfo property, int length)
        {
            _property = property;
            _length = length;
        }

        public string Message { get; set; }

        public int Length { get { return _length; } }

        public void Validate(object target, Notification notification)
        {
            var rawValue = _property.GetValue(target, null);
            if(rawValue == null)
            {
                return;
            }

            if(rawValue.ToString().Length > _length)
            {
                notification.RegisterMessage(_property, Message);
            }
        }
    }
}