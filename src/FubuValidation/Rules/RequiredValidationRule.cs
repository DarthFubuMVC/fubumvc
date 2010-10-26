using System.Reflection;

namespace FubuValidation.Rules
{
    public class RequiredValidationRule : IValidationRule
    {
        public static readonly string FIELD = "field";
        private readonly PropertyInfo _property;

        public RequiredValidationRule(PropertyInfo property)
        {
            _property = property;
        }

        public string Message { get; set; }

        public void Validate(object target, Notification notification)
        {
            var rawValue = _property.GetValue(target, null);
            if(rawValue == null || rawValue.ToString() == string.Empty)
            {
                notification
                    .RegisterMessage(_property, Message)
                    .AddSubstitution(FIELD, _property.Name);
            }
        }
    }
}