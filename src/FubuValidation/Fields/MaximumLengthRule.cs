using FubuCore.Reflection;
using FubuValidation.Strategies;

namespace FubuValidation.Fields
{
    public class MaximumLengthRule : IFieldValidationRule
    {
        public static readonly string LENGTH = "length";
        private readonly int _length;

        public MaximumLengthRule(int length)
        {
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        public void Validate(Accessor accessor, ValidationContext context)
        {
            var rawValue = accessor.GetValue(context.Target);
            if (rawValue != null && rawValue.ToString().Length > Length)
            {
                context.Notification.RegisterMessage(accessor, ValidationKeys.MAX_LENGTH)
                    .AddSubstitution(LENGTH, _length.ToString());
            }
        }
    }
}